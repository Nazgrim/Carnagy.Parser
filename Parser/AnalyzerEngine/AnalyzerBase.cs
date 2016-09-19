using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;

namespace AnalyzerEngine
{
    public class AnalyzerBase : IAnalyzer
    {
        private IBaseRepository Repository { get; set; }

        public AnalyzerBase(IBaseRepository repository)
        {
            Repository = repository;
        }

        public void Run()
        {
            Analyze();
            Сalculation();
        }

        private void Analyze()
        {
            var configuration = Repository.GetConfigurations();
            AnalyzeDealerConfiguration(configuration);
            AnalyzeAutotraderConfiguration(configuration);
        }

        private void AnalyzeDealerConfiguration(List<MainConfiguration> configuration)
        {
            var dealerConfiguration = configuration.Where(a => a.DealerId.HasValue);
            foreach (var mainConfiguration in dealerConfiguration)
            {
                foreach (var parssedCar in mainConfiguration.ParsedCars.Where(a => a.IsDeleted))
                {
                    var priceValue = int.Parse(parssedCar.Prices.OrderByDescending(a => a.DateTime).First().Value);
                    if (parssedCar.IsParsed)
                    {
                        parssedCar.AdvertCar.Car.Price = priceValue;
                        Repository.AddAdvertCarPrice(parssedCar.Id, priceValue);
                    }
                    else
                    {
                        parssedCar.IsParsed = true;
                        var advertCar = GetAdvertCar(parssedCar, priceValue, mainConfiguration.Dealer);
                        Repository.AddAdvertCar(advertCar);
                    }
                }

            }
        }

        private void AnalyzeAutotraderConfiguration(List<MainConfiguration> configuration)
        {
            var autotraderConfiguration = configuration.Where(a => a.DealerId.HasValue);

            foreach (var mainConfiguration in autotraderConfiguration)
            {
                foreach (var parssedCar in mainConfiguration.ParsedCars.Where(a => !a.IsDeleted))
                {
                    #region MoreDifficultWay
                    //create relation between dealer advert and autortrader advert 
                    //var priceValue = int.Parse(parssedCar.Prices.OrderByDescending(a => a.DateTime).First().Value);
                    //var dealer = GetDealer();
                    //if (dealer.Id == 0)
                    //{
                    //    Repository.CreateDealer(dealer);
                    //    var advertCar = GetAdvertCar(mainConfiguration, parssedCar, priceValue, dealer);
                    //    Repository.AddAdvertCar(advertCar);
                    //}
                    //else
                    //{
                    //    var car = GetCar();
                    //    //var advertCar=car.AdvertCars.w
                    //}
                    #endregion
                    var priceValue = int.Parse(parssedCar.Prices.OrderByDescending(a => a.DateTime).First().Value);
                    var advertCar = GetAdvertCar(parssedCar, priceValue);
                    Repository.AddAdvertCar(advertCar);
                }
            }
        }

        private void Сalculation()
        {
            var stockCars = Repository.GetStockCars();
            var timeStart= DateTime.Now;
            foreach (var stockCar in stockCars)
            {
                stockCar.StockCarPrices.Add(new StockCarPrice
                {
                    DateTime = timeStart,
                    Value = (int)stockCar.Cars.Average(a => a.Price)
                }); 
            }
            Repository.SaveChanges();
        }

        private Dealer GetDealer()
        {
            return new Dealer();
        }

        private string GetValue(IEnumerable<FieldValue> fieldValues, string key)
        {
            return fieldValues.SingleOrDefault(a => a.Field.Name == key)?.Value;
        }

        private T GetDictionaryEntity<T>(ParsedCar parssedCar, string key) where T : class, IDictionaryEntity, new()
        {
            var value = GetValue(parssedCar.FieldValues, key);
            if (value == null)
                return null;
            return Repository.GetDictionaryEntity<T>(value) ?? new T { Value = value };
        }

        private StockCar GetStockCar(ParsedCar parssedCar)
        {
            var maker = GetDictionaryEntity<Make>(parssedCar, FiledNameConstant.Make);
            var model = GetDictionaryEntity<Model>(parssedCar, FiledNameConstant.Model);
            var year = GetDictionaryEntity<Year>(parssedCar, FiledNameConstant.Year);
            var bodyType = GetDictionaryEntity<BodyType>(parssedCar, FiledNameConstant.BodyType);
            var styleTrim = GetDictionaryEntity<StyleTrim>(parssedCar, FiledNameConstant.StyleTrim);
            var drivetrain = GetDictionaryEntity<Drivetrain>(parssedCar, FiledNameConstant.Drivetrain);

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

            return stockCar;
        }

        private AdvertCar GetAdvertCar(ParsedCar parssedCar, int priceValue, Dealer dealer = null)
        {
            var stockCar = GetStockCar(parssedCar);
            var advertCar = new AdvertCar
            {
                Car = new Car
                {
                    StockCar = stockCar,
                    Dealer = dealer,
                    Price = priceValue
                },
                IsDealer = true,
                Url = parssedCar.Url,
                AdvertCarPrices = new List<AdvertCarPrice>
                            {
                                new AdvertCarPrice { Value = priceValue, DateTime = DateTime.Now }
                            }
            };
            return advertCar;
        }

        private Car GetCar()
        {
            return new Car();
        }
    }
}
