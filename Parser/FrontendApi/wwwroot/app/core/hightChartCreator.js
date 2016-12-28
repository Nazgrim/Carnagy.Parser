var hightChartCreatorModule = (function () {
    function overrideHightChart() {
        (function (H) {
            var each = H.each,
                pick = H.pick,
                mathAbs = Math.abs,
                mathMax = Math.max,
                mathMin = Math.min,
                mathCeil = Math.ceil;

            H.wrap(Highcharts.seriesTypes.column.prototype, 'getColumnMetrics', function (proceed, pointX) {
                var series = this,
                    options = series.options,
                    xAxis = series.xAxis,
                    yAxis = series.yAxis,
                    reversedXAxis = xAxis.reversed,
                    stackKey,
                    stackGroups = {},
                    columnIndex,
                    columnCount = 0;

                // Get the total number of column type series.
                // This is called on every series. Consider moving this logic to a
                // chart.orderStacks() function and call it on init, addSeries and removeSeries
                if (options.grouping === false) {
                    columnCount = 1;
                } else {
                    H.each(series.chart.series, function (otherSeries) {
                        var otherOptions = otherSeries.options,
                            otherYAxis = otherSeries.yAxis;
                        if (otherSeries.type === series.type && otherSeries.visible &&
                            yAxis.len === otherYAxis.len && yAxis.pos === otherYAxis.pos) {

                            var hasPointInX = false;
                            H.each(otherSeries.userOptions.data, function (point) {
                                if (point.x === pointX) hasPointInX = true;
                            });
                            if (hasPointInX) {

                                // #642, #2086
                                if (otherOptions.stacking) {
                                    stackKey = otherSeries.stackKey;
                                    if (stackGroups[stackKey] === UNDEFINED) {
                                        stackGroups[stackKey] = columnCount++;
                                    }
                                    columnIndex = stackGroups[stackKey];
                                } else if (otherOptions.grouping !== false) { // #1162
                                    columnIndex = columnCount++;
                                }
                                otherSeries.columnIndex = columnIndex;
                            }
                        }
                    });
                }

                var categoryWidth = mathMin(
                    mathAbs(xAxis.transA) * (xAxis.ordinalSlope || options.pointRange || xAxis.closestPointRange || xAxis.tickInterval || 1), // #2610
                    xAxis.len // #1535
                ),
                    groupPadding = categoryWidth * options.groupPadding,
                    groupWidth = categoryWidth - 2 * groupPadding,
                    pointOffsetWidth = groupWidth / columnCount,
                    pointWidth = mathMin(
                        options.maxPointWidth || xAxis.len,
                        pick(options.pointWidth, pointOffsetWidth * (1 - 2 * options.pointPadding))
                    ),
                    pointPadding = (pointOffsetWidth - pointWidth) / 2,
                    colIndex = (reversedXAxis ?
                        columnCount - (series.columnIndex || 0) : // #1251
                        series.columnIndex) || 0,
                    pointXOffset = pointPadding + (groupPadding + colIndex *
                        pointOffsetWidth - (categoryWidth / 2)) *
                        (reversedXAxis ? -1 : 1);

                // Save it for reading in linked series (Error bars particularly)
                return (series.columnMetrics = {
                    width: pointWidth,
                    offset: pointXOffset
                });
            });


            H.wrap(Highcharts.seriesTypes.column.prototype, 'translate', function (proceed) {
                var series = this,
                    chart = series.chart,
                    options = series.options,
                    borderWidth = series.borderWidth = pick(
                        options.borderWidth,
                        series.closestPointRange * series.xAxis.transA < 2 ? 0 : 1 // #3635
                    ),
                    yAxis = series.yAxis,
                    threshold = options.threshold,
                    translatedThreshold = series.translatedThreshold = yAxis.getThreshold(threshold),
                    minPointLength = pick(options.minPointLength, 5),
                    metrics, // = series.getColumnMetrics(1),
                    pointWidth, // = metrics.width,
                    seriesBarW, // = series.barW = mathMax(pointWidth, 1 + 2 * borderWidth), // postprocessed for border width
                    pointXOffset; // = series.pointXOffset = metrics.offset;

                if (chart.inverted) {
                    translatedThreshold -= 0.5; // #3355
                }

                H.Series.prototype.translate.apply(series);

                // Record the new values
                each(series.points, function (point) {
                    metrics = series.getColumnMetrics(point.x);
                    pointWidth = metrics.width;
                    seriesBarW = series.barW = mathMax(pointWidth, 1 + 2 * borderWidth); // postprocessed for border width
                    pointXOffset = series.pointXOffset = metrics.offset;

                    var yBottom = mathMin(pick(point.yBottom, translatedThreshold), 9e4), // #3575
                        safeDistance = 999 + mathAbs(yBottom),
                        plotY = mathMin(mathMax(-safeDistance, point.plotY), yAxis.len + safeDistance), // Don't draw too far outside plot area (#1303, #2241, #4264)
                        barX = point.plotX + pointXOffset,
                        barW = seriesBarW,
                        barY = mathMin(plotY, yBottom),
                        up,
                        barH = mathMax(plotY, yBottom) - barY;

                    // When the pointPadding is 0, we want the columns to be packed tightly, so we allow individual
                    // columns to have individual sizes. When pointPadding is greater, we strive for equal-width
                    // columns (#2694).
                    if (options.pointPadding) {
                        seriesBarW = mathCeil(seriesBarW);
                    }

                    // Handle options.minPointLength
                    if (mathAbs(barH) < minPointLength) {
                        if (minPointLength) {
                            barH = minPointLength;
                            up = (!yAxis.reversed && !point.negative) || (yAxis.reversed && point.negative);
                            barY = mathAbs(barY - translatedThreshold) > minPointLength ? // stacked
                                yBottom - minPointLength : // keep position
                                translatedThreshold - (up ? minPointLength : 0); // #1485, #4051
                        }
                    }

                    // Cache for access in polar
                    point.barX = barX;
                    point.pointWidth = pointWidth;

                    // Fix the tooltip on center of grouped columns (#1216, #424, #3648)
                    point.tooltipPos = chart.inverted ? [yAxis.len + yAxis.pos - chart.plotLeft - plotY, series.xAxis.len - barX - barW / 2, barH] : [barX + barW / 2, plotY + yAxis.pos - chart.plotTop, barH];

                    // Register shape type and arguments to be used in drawPoints
                    point.shapeType = 'rect';
                    point.shapeArgs = series.crispCol(barX, barY, barW, barH);
                });
            });
        } (Highcharts));
    }
    function getMarket() {
        return [
            {
                "category": { dealerId: 1, name: "Enclave", parentId: 0, hasSubCategories: false },
                "name": "Enclave",
                "data": [{
                    "y": 51,
                    "name": "Enclave",
                    "x": 0
                }]
            }, {
                "category": { dealerId: 2, name: "Encore", parentId: 0, hasSubCategories: true },
                "name": "Encore",
                "data": [{
                    "y": 49,
                    "name": "Encore",
                    "x": 1
                }]
            }, {
                "category": { dealerId: 3, name: "LaCrosse", parentId: 0, hasSubCategories: false },
                "name": "LaCrosse",
                "data": [{
                    "y": 52,
                    "name": "LaCrosse",
                    "x": 2
                }]
            }, {
                "category": { dealerId: 4, name: "Regal", parentId: 0, hasSubCategories: false },
                "name": "Regal",
                "data": [{
                    "y": 52,
                    "name": "Regal",
                    "x": 3
                }]
            }, {
                "category": { dealerId: 5, name: "Verano", parentId: 0, hasSubCategories: false },
                "name": "Verano",
                "data": [{
                    "y": 52,
                    "name": "Verano",
                    "x": 4
                }]
            }, {
                "category": { dealerId: 6, name: "AWD", parentId: 2, hasSubCategories: false },
                "name": "AWD",
                "data": [{
                    "y": 152,
                    "name": "AWD",
                    "x": 0
                }]
            }, {
                "category": { id: 7, name: "FWD", parentId: 2, hasSubCategories: false },
                "name": "FWD",
                "data": [{
                    "y": 102,
                    "name": "FWD",
                    "x": 1
                }]
            }];
    }
    function getDealer() {
        return [
            {
                "category": { name: "Dealer", parentId: 0, hasSubCategories: false },
                "name": "Dealer",
                "data": [
                    {
                        "name": "Dealer1",
                        "y": 50,
                        "x": 0
                    }, {
                        "name": "Dealer",
                        "y": 50,
                        "x": 1
                    }, {
                        "name": "Dealer",
                        "y": 50,
                        "x": 2
                    }, {
                        "name": "Dealer",
                        "y": 50,
                        "x": 3
                    }, {
                        "name": "Dealer",
                        "y": 50,
                        "x": 4
                    }],
                "color": "#FF0000"
            }, {
                "category": { name: "Dealer", parentId: 2, hasSubCategories: false },
                "name": "Dealer",
                "data": [
                    {
                        "name": "Dealer",
                        "y": 140,
                        "x": 0
                    }, {
                        "name": "Dealer",
                        "y": 110,
                        "x": 1
                    }],
                "color": "#FF0000"
            }];
    }

    function getData(parentId) {
        var result = [];
        var market = getMarket();
        var categories = [];
        for (var i = 0; i < market.length; i++) {
            if (market[i].category.parentId == parentId) {
                result.push(market[i]);
                categories.push(market[i].name);
            }
        }

        var dealer = getDealer();
        for (var i = 0; i < dealer.length; i++) {
            if (dealer[i].category.parentId == parentId) {
                result.push(dealer[i]);
            }
        }
        return { series: result, categories: categories };
    }

    var baseOption = {
        "options": {
            chart: {
                type: "column",
            },
            plotOptions: {
                bar: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        allowOverlap: true,
                    }
                },
                series: {
                    borderWidth: 0,
                    dataLabels: {
                        enabled: true,
                        format: '{point.y:.1f}$'
                    }
                }
            },
            exporting: { enabled: false }
        },
        title: {
            text: 'title'
        },
        subtitle: {
            text: 'subtitle'
        },
        xAxis: {
            type: 'category'
        },
        yAxis: {
            min: 0,
            title: {
                text: 'yAxix title'
            }
        },
        tooltip: {
            enabled: false,
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        }
    }

    return {
        overrideHightChart: function () {
            overrideHightChart();
        },
        setChart: function (chart, parentId) {
            var len = chart.series.length;
            chart.series = [];
            var series = getData(parentId);
            for (var i = 0; i < series.series.length; i++) {
                chart.series.push(series.series[i]);
            }
            chart.xAxis.categories = series.categories;
        },
        getOptions: function () {
            var abc = getData(0);
            baseOption.series = abc.series;
            baseOption.xAxis.categories = abc.categories;
            return baseOption;
        }
    }
} ());

