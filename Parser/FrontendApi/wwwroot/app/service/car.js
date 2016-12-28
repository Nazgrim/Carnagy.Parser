angular
    .module("carModule", []);
angular
    .module("carModule")
    .service("carService", function ($resource) {
        var baseUrl = '/dealercar';
        function getXaxisPlotLines(msrpPrice, avrPrice, dealerPrice) {
            return [
                {
                    value: msrpPrice,
                    width: 2,
                    color: '#FF0000',
                    label: {
                        text: 'MSRP price',
                        verticalAlign: 'middle',
                        textAlign: 'center'
                    },
                    zIndex: 5
                },
                {
                    value: avrPrice,
                    width: 2,
                    color: '#00FF00',
                    label: {
                        text: 'Avr. price',
                        verticalAlign: 'middle',
                        textAlign: 'center'
                    },
                    zIndex: 5
                },
                {
                    value: dealerPrice,
                    width: 2,
                    color: '#0000FF',
                    label: {
                        text: 'Your price',
                        verticalAlign: 'middle',
                        textAlign: 'center'
                    },
                    zIndex: 5
                }
            ]
        }
        function getXaxisPlotBands(seriesDataLength, min, max) {
            var xAxisPlotBands = [];
            var xAxisPlotBandsDefault = [
                {
                    chartValue: 'below',
                    labelBtext: 'Exceptional Price',
                    color: '#FCFFC5',
                },
                {
                    chartValue: 'great',
                    labelBtext: 'Great Price',
                    color: '#c4c336',
                },
                {
                    chartValue: 'good',
                    labelBtext: 'Good price',
                    color: '#CCFFC5',
                },
                {
                    chartValue: 'above',
                    labelBtext: 'Above Market',
                    color: '#4ac336'
                },
            ];
            //������� 
            //������ ���������� �� 1 �������� ������ � ������������� �� 1 �������� �����, 
            //���� �������� ������� ��� ��������� �� ������ �� �� ��������
            var step1 = ((max - min) / 20);
            min -= step1;
            max += step1;
            var step = Math.round((max - min) / xAxisPlotBandsDefault.length);
            var from = min;
            for (var i = 0; i < xAxisPlotBandsDefault.length; i++) {
                var to = from + step;
                var xAxisPlotBandDefault = xAxisPlotBandsDefault[i];
                var labelText = '<div><b>' + xAxisPlotBandDefault.labelBtext + '</b></div><div>Less than ' + Math.round(to) + '$';
                if (i == xAxisPlotBandsDefault.length - 1) {
                    labelText += ' or more';
                }
                labelText += '</div>';
                xAxisPlotBands.push({
                    from: from,
                    to: to,
                    color: xAxisPlotBandDefault.color,
                    label: {
                        text: labelText,
                        align: 'left',
                        useHTML: true,
                    }
                });
                from = to;
            }
            return xAxisPlotBands;
        }
        return {
            getInformationById: function (carId) {
                var DealerCar = $resource(baseUrl + '/information?carId=:carId', { carId: '@carId' });
                return DealerCar.get({ carId: carId });
            },
            getChartConfig: function ($scope) {
                var DealerCar = $resource(baseUrl + '/chartData?carId=:carId&stockCarId=:stockCarId&dealerId=:dealerId', { carId: '@carId', stockCarId: '@stockCarId', dealerId: '@dealerId' });
                return DealerCar.get({ carId: $scope.carId, stockCarId: $scope.stockCarId, dealerId: 1 })
                    .$promise
                    .then(function (chartData) {
                        var max = Math.ceil(chartData.max / 1000) * 1000;
                        var min = Math.floor(chartData.min / 1000) * 1000;
                        var step = (max - min) / 20;
                        var xAxisplotLines = getXaxisPlotLines(chartData.msrpPrice, chartData.avrPrice, chartData.dealerPrice);
                        var xAxisPlotBands = getXaxisPlotBands(chartData.seriesData.length, min, max);
                        var chartSeries = chartData.seriesData.map(function (e) {
                            return {
                                y: e.value,
                                carsId: e.carsId
                            }
                        });
                        return {
                            "options": {
                                plotOptions: {
                                    bar: {
                                        borderWidth: 0,
                                        dataLabels: {
                                            enabled: false,
                                            allowOverlap: false,
                                        }
                                    },
                                    series: {
                                        borderWidth: 0,
                                        dataLabels: {
                                            enabled: false
                                        },
                                        allowPointSelect: true,
                                        cursor: 'pointer',
                                        point: {
                                            events: {
                                                click: function (e) {
                                                    console.log(e);
                                                    console.log(this);
                                                    $scope.selectedLegend.people = this.y;
                                                    if (!this.selected) {
                                                        console.log('emit');
                                                        $scope.$emit('selectSeriesUp', this.carsId);
                                                    }
                                                    else {
                                                        console.log('emit');
                                                        $scope.$emit('selectSeriesUp', []);
                                                    }
                                                    $scope.selectedLegend.selected = !this.selected;
                                                    $scope.selectedLegend.from = this.series.data[this.index - 1].category;
                                                    $scope.selectedLegend.to = this.category;

                                                    $scope.$apply();
                                                },
                                            }
                                        }
                                    },
                                    column: {
                                        states: {
                                            hover: {
                                                color: '#000000'
                                            }
                                        }
                                    }
                                },
                                chart: {
                                    type: 'column',
                                    height: 400
                                },
                                legend: {
                                    enabled: false
                                },
                                exporting: {
                                    enabled: false
                                },
                            },
                            title: {
                                text: '',
                                style: {
                                    display: 'none'
                                }
                            },
                            subtitle: {
                                text: '',
                                style: {
                                    display: 'none'
                                }
                            },
                            credits: {
                                enabled: false
                            },
                            yAxis: {
                                title: {
                                    text: 'Count',
                                    //style: 'display:none'
                                }
                            },
                            xAxis: {
                                plotLines: xAxisplotLines,
                                plotBands: xAxisPlotBands,
                                startOnTick: true,
                                minPadding: 0,
                                min: min,
                                max: max,
                                tickInterval: step,
                                labels: {
                                    format: '$ {value}'
                                }
                            },
                            series: [
                                {
                                    name: 'Market',
                                    type: 'column',
                                    data: chartSeries,
                                    tooltip: {
                                        valueSuffix: ' count'
                                    },
                                    pointStart: min,
                                    pointInterval: step,
                                }
                            ]
                        };
                    });
            },
            getSimilarCars: function () {
                var DealerCar = $resource(baseUrl + '/similarcars/:delearId', { dealerCarId: '@id' });
                return DealerCar.query({ dealerCarId: 123 });
            },
            getPriceTrend: function (stockCarId) {
                var DealerCar = $resource(baseUrl + '/chartSeries?stockCarId=:stockCarId', { stockCarId: '@stockCarId' });
                return DealerCar.get({ stockCarId: stockCarId })
                    .$promise
                    .then(function (chartSeries) {
                        var result = {
                            options: {
                                chart: {
                                    type: 'line',
                                    height: 300
                                },
                                legend: {
                                    enabled: true
                                },
                                exporting: {
                                    enabled: false
                                }
                            },
                            title: {
                                text: '',
                                style: {
                                    display: 'none'
                                }
                            },
                            subtitle: {
                                text: '',
                                style: {
                                    display: 'none'
                                }
                            },
                            credits: {
                                enabled: false
                            },
                            series: [{
                                name: chartSeries.name,
                                data: chartSeries.data,
                                carId: chartSeries.carId
                            }],
                            xAxis: { type: 'datetime' },
                            yAxis: {
                                title: {
                                    text: '',
                                    style: {
                                        display: 'none'
                                    }
                                }
                            }
                        };
                        return result;
                    });
            },
            getDealerCompetitors: function (stockCarId) {
                var DealerCar = $resource(baseUrl + '/dealercompetitors?stockCarId=:stockCarId', { stockCarId: '@id' });
                return DealerCar.query({ stockCarId: stockCarId })
            }
        };
    })