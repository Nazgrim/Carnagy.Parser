using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using DataAccess.Repositories;
using FrontendApi.Helpers;
using FrontendApi.Models;
using Dealer = FrontendApi.Models.Dealer;

namespace FrontendApi.Service
{
    public class DealerService : IDealerService
    {
        private IAnalyseRepository Repository { get; set; }

        public DealerService(IAnalyseRepository repository)
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
                imgScr = car.Id.ToString(),
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
                },
                msrpPrice = new DelalerClassPrice
                {
                    value = car.MsrpPrice.ToString(),
                    date = "From last Week",
                    difference = "34%"
                }
            };
            return dealerClassInformation;
        }

        public ChartData GetChartDataById(int stockCarId, int dealerId, int carId, string location)
        {
            var stockCar = Repository.GetStockCar(a => a.Id == stockCarId);
            Func<Car, bool> filter = car => car.StockCar.YearId == stockCar.YearId
            && car.StockCar.MakeId == stockCar.MakeId
            && car.StockCar.ModelId == stockCar.ModelId
            && car.StockCar.BodyTypeId == stockCar.BodyTypeId
            && car.StockCar.DrivetrainId == stockCar.DrivetrainId
            && car.StockCar.StyleTrimId == stockCar.StyleTrimId;

            //if (!string.IsNullOrWhiteSpace(location))
            //{
            //    filter = car => car.StockCar.YearId == stockCar.YearId
            //                 && car.StockCar.MakeId == stockCar.MakeId
            //                 && car.StockCar.ModelId == stockCar.ModelId
            //                 && car.StockCar.BodyTypeId == stockCar.BodyTypeId
            //                 && car.StockCar.DrivetrainId == stockCar.DrivetrainId
            //                 && car.StockCar.StyleTrimId == stockCar.StyleTrimId
            //                 && car.Dealer.Province == location;
            //}

            var cars = Repository.GetCarsByFilter(filter);
            var dealer = cars.FirstOrDefault(a => a.Id == carId);
            var abc = cars.Select(a => a.Dealer.Location).ToList();
            if (!string.IsNullOrWhiteSpace(location))
            {
                cars = cars.Where(a => a.Dealer.Location.Split(',')[1].Contains(location)).ToList();
            }



            return GetChartData(cars, dealer, stockCar);
        }

        private ChartData GetChartData(List<Car> cars, Car dealer, StockCar stockCar)
        {
            if (!cars.Any())
            {
                return new ChartData
                {
                    avrPrice = 0,
                    max = 0,
                    min = 0,
                    dealerPrice = dealer == null ? 0 : dealer.Price,
                    msrpPrice = dealer == null ? 0 : dealer.MsrpPrice,
                    seriesData = Enumerable.Empty<SeriesDataValue>()
                };
            }
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
                msrpPrice = dealer.MsrpPrice,
                seriesData = seriesData
            };
            return chartData;
        }

        public List<DealerCompetitorCar> GetDealerCompetitorsById(int stockCarId, int dealerId, int carId)
        {
            var car = Repository.GetCarById(carId) ?? new Car();
            var result = Repository.GetCarsByStockCarId(stockCarId)
                .Select(c => new DealerCompetitorCar
                {
                    url = c.Url,
                    price = c.Price,
                    dealerName = c.DealerId == dealerId ? "You" : c.Dealer.Name,
                    dealerId = c.DealerId,
                    id = c.Id,
                    isDealerCar = c.DealerId == dealerId,
                    priceDifference = c.Price - car.Price,
                    city = c.Dealer.CityName ?? "No information",
                    province = c.Dealer.Province ?? "No information",
                    createdTime = c.CreatedTime.ToShortDateString(),

                    dealerLocation = c.Dealer.Location,
                    year = c.StockCar.Year.Value,
                    model = c.StockCar.Model.Value,
                    bodyType = c.StockCar.BodyType.Value,
                    drivetrain = c.StockCar.Drivetrain.Value,
                    make = c.StockCar.Make.Value,
                    styleTrim = c.StockCar.StyleTrim.Value

                }).ToList();
            return result;
        }

        public ChartSeries GetPriceTrendById(int stockCarId, int carId, string location, IEnumerable<int> carsId)
        {
            var cars = Repository.GetCarsByStockCarId(stockCarId);
            var dealerCar = cars.FirstOrDefault(a => a.Id == carId);
            var stockCar = Repository.GetStockCarWithPrices(stockCarId);
            var year = stockCar.Year.Value;
            var make = stockCar.Make.Value;
            var model = stockCar.Model.Value;

            if (!string.IsNullOrWhiteSpace(location))
            {
                cars = cars.Where(a => a.Dealer.Province == location).ToList();
            }

            if (carsId != null && carsId.Any())
            {
                cars = cars.Where(a => carsId.Contains(a.Id)).ToList();
            }
            if (cars.Count == 0)
            {
                return new ChartSeries
                {
                    carId = stockCarId,
                    name = $"{year} {make} {model}",
                    data = new List<long[]>(),
                    msrpPrice = 0
                };
            }

            var list = new List<AdvertCarPrice>();
            foreach (var car in cars)
            {

                var advertsCar = car.MainAdvertCar.AdvertCars.SingleOrDefault(a => a.IsDealer) ??
                                  car.MainAdvertCar.AdvertCars.FirstOrDefault(a => !a.IsDealer);
                list.AddRange(advertsCar.AdvertCarPrices);
            }

            var group = list.GroupBy(a => a.DateTime.ToShortDateString());

            

            var chartSeties = new ChartSeries
            {
                carId = stockCarId,
                name = $"{year} {make} {model}",
                data = group                  
                    .Select(a => new[] { DateTime.Parse(a.Key).ConvertToUnixTime(), (long)a.Average(b => b.Value) })
                    .OrderBy(a => a[0])
                    .ToList(),
                msrpPrice = dealerCar.MsrpPrice
            };

            return chartSeties;
        }

        public DealerInformation GetDealer(int id)
        {
            var dealer = Repository.GetDealerById(id);
            var dealerCars = new DealerInformation()
            {
                logo = dealer.Logo,
                url = dealer.WebSireUrl,
                name = dealer.Name,
                phone = "905-238-2886",
                adress = dealer.Location,
                priceCars = dealer.Cars.Sum(a => a.Price),
                carCount = dealer.Cars.Count(),
                webSiteName = dealer.WebSiteName,
            };
            return dealerCars;
        }

        public ChartSeries GetCountTrendById(int stockCarId, string location)
        {
            var cars = Repository.GetCarsByStockCarId(stockCarId);
            var car1 = cars.First().StockCar;
            var year = car1.Year.Value;
            var make = car1.Make.Value;
            var model = car1.Model.Value;
            if (!string.IsNullOrWhiteSpace(location))
            {
                cars = cars.Where(a => a.Dealer.Province == location).ToList();
            }
            if (cars.Count == 0)
            {
                return new ChartSeries
                {
                    carId = stockCarId,
                    name = $"{year} {make} {model}",
                    data = new List<long[]>()
                };
            }
            var dic = new Dictionary<DateTime, int>();
            foreach (var car in cars)
            {
                var crteadTime = car.CreatedTime.Date;
                var stopDate = (car.DeletedTime ?? DateTime.Now).Date;
                for (int i = 0; i < (stopDate - crteadTime).Days; i++)
                {
                    var curTime = crteadTime.AddDays(i);
                    if (!dic.ContainsKey(curTime))
                    {
                        dic.Add(curTime, 0);
                    }
                    dic[curTime]++;
                }
            }



            var chartSeties = new ChartSeries
            {
                carId = stockCarId,
                name = $"{year} {make} {model}",
                data = dic
                   .Select(a => new[] { a.Key.ConvertToUnixTime(), a.Value })
                   .OrderBy(a => a[0])
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
                        carsId = filtered.Select(a => a.Id).ToList()
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