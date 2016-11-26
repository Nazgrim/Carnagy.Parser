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
                .Where(a => a.StockCar.Make.Value == "Buick")
                .Select(a => new CarViewModel()
                {
                    year = a.StockCar.Year.Value,
                    make = a.StockCar.Make.Value,
                    model = a.StockCar.Model.Value,
                    bodyType = a.StockCar.BodyType.Value,
                    styleTrim = a.StockCar.StyleTrim.Value,
                    drivetrain = a.StockCar.Drivetrain.Value,
                    price = a.Price.ToString(),
                    id = a.Id,
                    stockCarId = a.StockCarId,
                    url = a.Url
                })
                .ToList();
        }

        public DealerClassInformation GetInformationById(int carId)
        {
            var car = Repository.GetCarById(carId);
            var year = car.StockCar.Year.Value;
            var make = car.StockCar.Make.Value;
            var model = car.StockCar.Model.Value;
            var bodyType = car.StockCar.BodyType.Value;
            var styleTrim = car.StockCar.StyleTrim.Value;
            var drivetrain = car.StockCar.Drivetrain.Value;
            var dealerClassInformation = new DealerClassInformation
            {
                name = $"{year} {make} {model}",
                imgScr = $"http://localhost/WepApi/image/cars/{car.Id}.jpg",
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
                    value = car.StockCar.Price.ToString(),
                    date = "From last Week",
                    difference = "34%"
                }
            };
            return dealerClassInformation;
        }

        public ChartData GetChartDataById(int stockCarId, int dealerId)
        {
            var stockCar = Repository.GetStockCar(a => a.Id == stockCarId);
            var cars = Repository.GetStockCarPrices(stockCarId);
            var dealer = cars.FirstOrDefault(a => a.DealerId == dealerId);
            var carsPrice = cars.Select(a => a.Price);
            var max = Math.Ceiling(carsPrice.Max() / 1000) * 1000;
            var min = Math.Floor(carsPrice.Min() / 1000) * 1000;
            var seriesData = GetSeriesData(cars, max, min);
            var chartData = new ChartData
            {
                avrPrice = (int)carsPrice.Average(),
                max = max,
                min = min,
                dealerPrice = dealer.Price,
                msrpPrice = stockCar.MsrpPrice,
                seriesData = seriesData
            };

            return chartData;
        }

        public List<DealerCompetitorCar> GetDealerCompetitorsById(int stockCarId, int dealerId)
        {
            var result = Repository.GetCarsByStockCarId(stockCarId)
                .Select(c => new DealerCompetitorCar
                {
                    url = c.Url,
                    year = c.StockCar.Year.Value,
                    model = c.StockCar.Model.Value,
                    bodyType = c.StockCar.BodyType.Value,
                    drivetrain = c.StockCar.Drivetrain.Value,
                    make = c.StockCar.Make.Value,
                    styleTrim = c.StockCar.StyleTrim.Value,
                    price = c.Price,
                    dealerLocation = c.Dealer.Location,
                    dealerName = c.Dealer.Name,
                    dealerId = c.DealerId
                }).ToList();
            return result;
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

        private IEnumerable<SeriesDataValue> GetSeriesData(List<Car> carDeales, double max, double min)
        {
            if (carDeales.Count <= 1)
            {
                return new List<SeriesDataValue> { new SeriesDataValue { value = 1 } };
            }

            var minMaxDif = max - min;
            var areaCount = 4;//количество областей
            carDeales = carDeales.OrderBy(a => a.Price).ToList();
            var increment = minMaxDif / areaCount;
            var previousValue = min;
            var result = new List<SeriesDataValue>();
            for (var i = min + increment; i <= max; i += increment)
            {
                var areaCars = carDeales.Where(a => a.Price > previousValue && a.Price <= i)
                    .OrderBy(a => a.Price)
                    .ToList();
                if (!areaCars.Any())
                {
                    previousValue = i;
                    result.Add(new SeriesDataValue { value = 0 });
                    continue;
                }

                var areaCountX = 5;//количество областей
                var incrementX = increment / areaCountX;
                var previousValueX = previousValue;
                for (var j = previousValue + incrementX; j <= i; j += incrementX)
                {
                    var filtered = areaCars.Where(a => a.Price > previousValueX && a.Price <= j);
                    result.Add(new SeriesDataValue
                    {
                        value = filtered.Count(),
                        dealersId = filtered.Select(a => a.DealerId).ToList()
                    });
                    previousValueX = j;
                }

                previousValue = i;

            }
            return result;
        }

        #region ForDevelopOnly
        public void InitDb(Dealer dealer)
        {
            foreach (var carViewModel in dealer.cars)
            {
                var maker = Repository.GetMakersByValue(carViewModel.bodyType) ??
                            new Make { Value = carViewModel.make };

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

                var stockCar = Repository.GetStockCar(filter) ?? new StockCar
                {
                    Year = year,
                    Make = maker,
                    Model = model,
                    BodyType = bodyType,
                    Drivetrain = drivetrain,
                    StyleTrim = styleTrim,
                };
                var car = new Car
                {
                    DealerId = 1,
                    StockCar = stockCar,
                    Url = carViewModel.url
                };
                var price = 0;
                int.TryParse(carViewModel.dealerAdvertisedPrice, out price);
                car.Price = price;
                Repository.AddCar(car);
            }
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