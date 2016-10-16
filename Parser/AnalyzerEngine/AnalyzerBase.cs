using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Utility;

namespace AnalyzerEngine
{
    public class AnalyzerBase : IAnalyzer
    {
        private IBaseRepository Repository { get; set; }
        private IDownloadImage DownloadImager { get; set; }

        public AnalyzerBase(IBaseRepository repository, IDownloadImage downloadImager)
        {
            Repository = repository;
            DownloadImager = downloadImager;
        }

        public void Run()
        {
            Analyze();
            Сalculation();
        }
        private void Analyze()
        {
            var images = new List<ImageDownloadCommand>();
            var configuration = Repository.GetConfigurations();
            //AnalyzeDealerConfiguration(configuration);
            var autotraderImages = AnalyzeAutotraderConfiguration(configuration);
            images.AddRange(autotraderImages);
            DownloadImager.Download("Cars", images.Where(a => !string.IsNullOrWhiteSpace(a.Url)));
        }

        private List<ImageDownloadCommand> AnalyzeDealerConfiguration(List<MainConfiguration> configuration)
        {
            var result = new List<ImageDownloadCommand>();
            var now = DateTime.Now;
            var dealerConfiguration = configuration.Where(a => a.DealerId.HasValue);
            foreach (var mainConfiguration in dealerConfiguration)
            {
                foreach (var parssedCar in mainConfiguration.ParsedCars.Where(a => a.IsDeleted))
                {
                    var priceValue = int.Parse(parssedCar.Prices.OrderByDescending(a => a.DateTime).First().Value);
                    if (parssedCar.IsParsed)
                    {
                        parssedCar.AdvertCars.First().Car.Price = priceValue;
                        Repository.AddAdvertCarPrice(parssedCar.Id, priceValue, now);
                    }
                    else
                    {
                        parssedCar.IsParsed = true;
                        var advertCar = GetAdvertCar(parssedCar, priceValue, mainConfiguration.Dealer);
                        Repository.CreateAdvertCar(advertCar);
                    }
                }
            }
            return result;
        }

        private List<ImageDownloadCommand> AnalyzeAutotraderConfiguration(List<MainConfiguration> configuration)
        {
            var result = new List<ImageDownloadCommand>();
            var autotraderConfiguration = configuration.Where(a => !a.DealerId.HasValue);

            foreach (var mainConfiguration in autotraderConfiguration)
            {
                foreach (var parssedCar in mainConfiguration.ParsedCars.Where(a => !a.IsDeleted))
                {
                    var dealerName = GetValue(parssedCar.FieldValues, FiledNameConstant.DealerName); ;
                    if (dealerName == null)
                        continue;

                    var dealer = GetOrCreateDealer(parssedCar, dealerName);

                    #region MoreDifficultWay
                    //create relation between dealer advert and autortrader advert 
                    //var priceValue = int.Parse(parssedCar.Prices.OrderByDescending(a => a.DateTime).First().Value);

                    //if (dealer.Id == 0)
                    //{
                    //    Repository.CreateDealer(dealer);
                    //    var advertCar = GetDealerAdvertCar(mainConfiguration, parssedCar, priceValue, dealer);
                    //    Repository.CreateAdvertCar(advertCar);
                    //}
                    //else
                    //{
                    //    var car = GetCar();
                    //    //var advertCar=car.AdvertCars.w
                    //}
                    #endregion

                    var price = parssedCar.Prices
                        .OrderByDescending(a => a.DateTime)
                        .FirstOrDefault()?.Value ?? string.Empty;

                    var priceValue = 0.0;
                    if (!double.TryParse(
                        price,
                        NumberStyles.AllowCurrencySymbol |
                        NumberStyles.AllowDecimalPoint |
                        NumberStyles.AllowThousands,
                        new CultureInfo("en-US"),
                        out priceValue))
                        continue;

                    var advertCar = GetAdvertCar(parssedCar, priceValue, dealer);
                    if (advertCar != null)
                    {
                        Repository.CreateAdvertCar(advertCar);
                        result.Add(new ImageDownloadCommand { Id = advertCar.CarId, Url = GetValue(parssedCar.FieldValues, FiledNameConstant.ImgPath) });
                    }
                    else
                    {
                        Console.WriteLine("Не удалось распарсить");
                    }
                }
            }
            return result;
        }

        private void Сalculation()
        {
            var stockCars = Repository.GetStockCars();
            var timeStart = DateTime.Now;
            foreach (var stockCar in stockCars)
            {
                if (!stockCar.Cars.Any()) continue;
                var averagePrice = (int)stockCar.Cars.Average(a => a.Price);
                stockCar.Price = averagePrice;
                stockCar.StockCarPrices.Add(new StockCarPrice
                {
                    DateTime = timeStart,
                    Value = averagePrice
                });
            }
            Repository.SaveChanges();
        }

        private Dealer GetOrCreateDealer(ParsedCar parssedCar, string dealerName)
        {
            var dealer = Repository.GetDealerByName(dealerName);

            if (dealer != null) return dealer;

            var dealerLogo = GetValue(parssedCar.FieldValues, FiledNameConstant.DealerLogo);
            var dealerPlace = GetValue(parssedCar.FieldValues, FiledNameConstant.DealerPlace);
            dealer = new Dealer
            {
                Location = dealerPlace,
                Logo = dealerLogo,
                Name = dealerName,
                IsCreated = false
            };
            Repository.CreateDealer(dealer);

            return dealer;
        }

        private string GetValue(IEnumerable<FieldValue> fieldValues, string key)
        {
            return fieldValues.SingleOrDefault(a => a.Field.Name == key)?.Value;
        }
        private T GetDictionaryOrCreateEntity<T>(string value) where T : class, IDictionaryEntity, new()
        {
            var dictionary = Repository.GetDictionaryEntity<T>(value);
            if (dictionary != null)
                return dictionary;

            dictionary = new T { Value = value };
            Repository.CreateDictionary(dictionary);

            return dictionary;
        }
        private T GetDictionaryOrCreateEntity<T>(ParsedCar parssedCar, string key) where T : class, IDictionaryEntity, new()
        {
            var value = GetValue(parssedCar.FieldValues, key);
            if (value == null)
                return null;
            return Repository.GetDictionaryEntity<T>(value) ?? new T { Value = value };
        }

        private StockCar GetStockCar(ParsedCar parssedCar)
        {
            var nameFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Name);
            var makeFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Make);
            var modelFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Model);
            var yearFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Year);
            var bodyTypeFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.BodyType);
            var styleTrimFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.StyleTrim);
            var drivetrainFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Drivetrain);

            if (makeFieldValue == null)
                makeFieldValue = GetMakeValue(nameFieldValue);

            if (modelFieldValue == null)
                modelFieldValue = GetModelValue(nameFieldValue);

            if (yearFieldValue == null)
                yearFieldValue = GetYearValue(nameFieldValue);

            if (makeFieldValue == null ||
                modelFieldValue == null ||
                yearFieldValue == null ||
                bodyTypeFieldValue == null ||
                styleTrimFieldValue == null ||
                drivetrainFieldValue == null)
                return null;

            var make = GetDictionaryOrCreateEntity<Make>(makeFieldValue);
            var model = GetDictionaryOrCreateEntity<Model>(modelFieldValue);
            var year = GetDictionaryOrCreateEntity<Year>(yearFieldValue);
            var bodyType = GetDictionaryOrCreateEntity<BodyType>(bodyTypeFieldValue);
            var styleTrim = GetDictionaryOrCreateEntity<StyleTrim>(styleTrimFieldValue);
            var drivetrain = GetDictionaryOrCreateEntity<Drivetrain>(drivetrainFieldValue);

            Func<StockCar, bool> filter = a => a.YearId == year.Id &&
                                                a.MakeId == make.Id &&
                                                a.ModelId == model.Id &&
                                                a.BodyTypeId == bodyType.Id &&
                                                a.StyleTrimId == styleTrim.Id &&
                                                a.DrivetrainId == drivetrain.Id;

            var stockCar = Repository.GetStockCar(filter) ?? new StockCar
            {
                Year = year,
                Make = make,
                Model = model,
                BodyType = bodyType,
                Drivetrain = drivetrain,
                StyleTrim = styleTrim,
            };

            return stockCar;
        }

        private string GetYearValue(string name)
        {
            var years = Repository.GetDictionaryEntity<Year>();
            foreach (var year in years)
            {
                if (name.Contains(year.Value))
                {
                    return year.Value;
                }
            }
            return null;
        }

        private string GetModelValue(string name)
        {
            var models = Repository.GetDictionaryEntity<Model>();
            foreach (var model in models)
            {
                if (name.Contains(model.Value))
                {
                    return model.Value;
                }
            }
            return null;
        }

        private string GetMakeValue(string name)
        {
            var makes = Repository.GetDictionaryEntity<Make>();
            foreach (var make in makes)
            {
                if (name.Contains(make.Value))
                {
                    return make.Value;
                }
            }
            return null;
        }

        private AdvertCar GetAdvertCar(ParsedCar parssedCar, double priceValue, Dealer dealer = null)
        {
            var stockCar = GetStockCar(parssedCar);
            if (stockCar == null)
            {
                parssedCar.Status = ParsedCarStatus.AnalyzeError;
                Repository.SaveChanges();
                return null;
            }

            var advertCar = new AdvertCar
            {
                Car = new Car
                {
                    StockCar = stockCar,
                    Dealer = dealer,
                    Price = priceValue,
                    Url = parssedCar.Url
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
