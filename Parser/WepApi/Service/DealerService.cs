using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using DataAccess.Repositories;
using WepApi.Models;
using Dealer = WepApi.Models.Dealer;
using System;
using WepApi.Helpers;

namespace WepApi.Service
{
    public class DealerService : IDealerService
    {
        private IBaseRepository Repository { get; set; }
        public DealerService(IBaseRepository repository)
        {
            Repository = repository;
        }

        public List<CarViewModel> GetCarsByDealerId(int dealerId)
        {
            return Repository
                .GetCarsByDealerId(dealerId)
                .Select(a => new CarViewModel()
                {
                    year = a.StockCar.Year.Value,
                    make = a.StockCar.Make.Value,
                    model = a.StockCar.Model.Value,
                    bodyType = a.StockCar.BodyType.Value,
                    styleTrim = a.StockCar.StyleTrim.Value,
                    drivetrain = a.StockCar.Drivetrain.Value,
                    price = a.Price.ToString()
                })
                .ToList();
        }

        public DealerClassInformation GetInformationById(int carId)
        {
            var car = Repository.GetCarById(carId);
            var year = car.StockCar.Year.Value;
            var make = car.StockCar.Make.Value;
            var model = car.StockCar.Model.Value;
            var bodyType = car.StockCar.Model.Value;
            var styleTrim = car.StockCar.Model.Value;
            var drivetrain = car.StockCar.Model.Value;
            var dealerClassInformation = new DealerClassInformation
            {
                name = $"{year} {make} {model}",
                imgScr = car.StockCar.ImageScr,
                parametrs = new Dictionary<string, string>
                {
                    {"Bodytype", bodyType},
                    {"Trim", styleTrim},
                    {"Drivetrain", drivetrain}
                },
                dealerPrice = new DelalerClassPrice
                {
                    value = car.Price.ToString(),
                    date = "From last Week",
                    difference = "34%"
                },
                averagePrice = new DelalerClassPrice
                {
                    value = "7,325",
                    date = "From last Week",
                    difference = "34%"
                }
            };
            return dealerClassInformation;
        }

        public ChartData GetChartDataById(int dealerCarId)
        {
            var cars = GetCars();
            var chartData = new ChartData
            {
                avrPrice = (int)cars.Average(),
                max = cars.Max(),
                min = cars.Min(),
                dealerPrice = 17900,
                msrpPrice = 12400
            };
            chartData.seriesData = GetSeriesData(cars, chartData.max, chartData.min);

            return chartData;
        }

        public List<DealerCompetitor> GetDealerCompetitorsById(int stockCarId, int dealerId)
        {
            return Repository.GetDealers(a => a.Id != dealerId && a.Cars.Any(b => b.StockCarId == stockCarId))
                .Select(a => new DealerCompetitor
                {
                    url = a.WebSireUrl,
                    webSiteName = a.WebSiteName,
                    city = a.Location,
                    name = a.Name,
                    cars = a.Cars.Select(c => new DealerCompetitorCar
                    {
                        url = c.Url,
                        year = c.StockCar.Year.Value,
                        model = c.StockCar.Model.Value,
                        bodyType = c.StockCar.BodyType.Value,
                        drivetrain = c.StockCar.Drivetrain.Value,
                        maker = c.StockCar.Make.Value,
                        styleTrim = c.StockCar.StyleTrim.Value,
                        price = new CarPrice
                        {
                            value = c.Price.ToString(),
                            difference = 500,
                        }
                    })
                    .ToList()
                })
                .ToList();
        }

        public ChartSeries GetPriceTrendById(int stockCarId)
        {
            var car = Repository.GetStockCarWithPrices(stockCarId);
            var year = car.Year.Value;
            var make = car.Make.Value;
            var model = car.Model.Value;

            var chartSeties = new ChartSeries
            {
                carId = stockCarId,
                name = $"{year} {make} {model}",
                data = car.StockCarPrices
                    .Select(a => new[] { a.DateTime.ConvertToUnixTime(), a.Value })
                    .ToList()
            };

            return chartSeties;
        }

        #region ForDevelopOnly
        public void InitBd(Dealer dealer)
        {
            foreach (var carViewModel in dealer.cars)
            {
                var maker = Repository.GetMakersByValue(carViewModel.bodyType) ??
                            new Make { Value = carViewModel.bodyType };

                var model = Repository.GetModelsByValue(carViewModel.model) ??
                            new Model { Value = carViewModel.model };

                var year = Repository.GetYearByValue(carViewModel.year) ??
                                new Year { Value = carViewModel.year };

                var bodyType = Repository.GetBodyTypeByValue(carViewModel.bodyType) ??
                               new BodyType { Value = carViewModel.bodyType };

                var styleTrim = Repository.GetStyleTrimsByValue(carViewModel.styleTrim) ??
                                new StyleTrim { Value = carViewModel.styleTrim };

                var drivetrain = Repository.GetDrivetrainsByValue(carViewModel.drivetrain) ??
                                 new Drivetrain { Value = carViewModel.drivetrain };

                Func<StockCar, bool> filter = a => a.YearId == year.Id &&
                a.MakeId == maker.Id &&
                a.ModelId == model.Id &&
                a.BodyTypeId == bodyType.Id &&
                a.StyleTrimId == styleTrim.Id &&
                a.DrivetrainId == drivetrain.Id;

                var car = new Car { DealerId = 1 };
                var stockCar = Repository.GetStockCar(filter) ?? new StockCar
                {
                    Year = year,
                    Make = maker,
                    Model = model,
                    BodyType = bodyType,
                    Drivetrain = drivetrain,
                    StyleTrim = styleTrim,
                };
                car.StockCar = stockCar;
                car.Url = carViewModel.url;
                var price = 0;
                int.TryParse(carViewModel.dealerAdvertisedPrice, out price);
                car.Price = price;
                Repository.AddCar(car);
            }
        }

        private IEnumerable<int> GetSeriesData(List<int> carDeales, int max, int min)
        {
            var minMaxDif = max - min;
            var areaCount = 4;//количество областей
            carDeales = carDeales.OrderBy(a => a).ToList();
            var increment = minMaxDif / areaCount;
            var previousValue = min;
            var result = new List<int>();
            for (int i = min + increment; i <= max; i += increment)
            {
                var areaCars = carDeales.Where(a => a > previousValue && a <= i).OrderBy(a => a).ToList();
                if (!areaCars.Any())
                    continue;

                var minX = areaCars.Min();
                var maxX = areaCars.Max();
                var minMaxDifX = maxX - minX;
                var areaCountX = 4;//количество областей
                var incrementX = minMaxDifX / areaCountX;
                var previousValueX = minX;
                for (int j = minX + incrementX; j <= maxX; j += incrementX)
                {
                    var count = areaCars.Count(a => a > previousValueX && a <= i);
                    result.Add(count);
                    previousValueX = j;
                }

                previousValue = i;
            }
            return result;
        }

        private List<int> GetCars()
        {
            var carDeales = new List<int>();
            var rnd = new System.Random();
            for (int i = 0; i < 40; i++)
            {
                carDeales.Add(rnd.Next(12200, 23200));
            }
            return carDeales;
        }

        #endregion
    }
}