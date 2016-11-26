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

        private const int ParrsedCarByTimes = 1000;

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

        //private List<ImageDownloadCommand> AnalyzeDealerConfiguration(List<MainConfiguration> configuration)
        //{
        //    var result = new List<ImageDownloadCommand>();
        //    var now = DateTime.Now;
        //    var dealerConfiguration = configuration.Where(a => a.DealerId.HasValue);
        //    foreach (var mainConfiguration in dealerConfiguration)
        //    {
        //        foreach (var parssedCar in mainConfiguration.ParsedCars.Where(a => a.IsDeleted))
        //        {
        //            var priceValue = int.Parse(parssedCar.Prices.OrderByDescending(a => a.DateTime).First().Value);
        //            if (parssedCar.IsParsed)
        //            {
        //                parssedCar.AdvertCars.First().MainAdvertCar.Car.Price = priceValue;
        //                Repository.AddAdvertCarPrice(parssedCar.Id, priceValue, now);
        //            }
        //            else
        //            {
        //                parssedCar.IsParsed = true;
        //                var advertCar = GetOrCreateAdvertCar(parssedCar, priceValue, mainConfiguration.Dealer);
        //                Repository.CreateAdvertCar(advertCar);
        //            }
        //        }
        //    }
        //    return result;
        //}

        private List<ImageDownloadCommand> AnalyzeAutotraderConfiguration(List<MainConfiguration> configuration)
        {
            var result = new List<ImageDownloadCommand>();
            var autotraderConfiguration = configuration.Where(a => !a.DealerId.HasValue);
            var now = DateTime.Now;
            //На данный момент у нас всего один аггрегатор(autotrader), но в будущем их будет больше
            foreach (var mainConfiguration in autotraderConfiguration)
            {
                //мы не можем сразу забрать все сущности из базы которые необходимо обработать, поэтому разбиваем их на раввные порции
                //с помощью константы ParrsedCarByTimes, цикл while можно заменить на for но нужно будет делать запрос на текущие количество элементов
                //цикл выполняется пока возвращаются значения 
                var page = 0;
                while (true)
                {

                    //получаем порцию данных с которой дальше будем работать
                    var parsedCars = Repository.GetParsedCarsByPage(page * ParrsedCarByTimes, ParrsedCarByTimes, mainConfiguration.Id);

                    //если данных нет выходим из цикла
                    if (!parsedCars.Any())
                    {
                        break;
                    }
                    page++;

                    foreach (var parsedCar in parsedCars)
                    {
                        //без диллера некуда прикрепить машину.
                        var dealerWebSite = GetValue(parsedCar.FieldValues, FiledNameConstant.DealerWebSite);
                        if (dealerWebSite == null)
                        {
                            parsedCar.Status = ParsedCarStatus.NoDealerWebSite;
                            Repository.SaveChanges();
                            continue;
                        }

                        var dealer = GetOrCreateDealer(parsedCar, dealerWebSite);

                        var price = parsedCar.Prices
                            .OrderByDescending(a => a.DateTime)
                            .FirstOrDefault()?.Value ?? string.Empty;
                        
                        //TODO: выбрасывать эти машины не стоит, но нужно учитывать что они могут нарушить логику работы расчетов
                        var priceValue = 0.0;
                        if (!double.TryParse(
                            price,
                            NumberStyles.AllowCurrencySymbol |
                            NumberStyles.AllowDecimalPoint |
                            NumberStyles.AllowThousands,
                            new CultureInfo("en-US"),
                            out priceValue))
                        {
                            parsedCar.Status = ParsedCarStatus.CannotParsePrice;
                            Repository.SaveChanges();
                            continue;
                        }
                        
                        var stockCar = GetStockCar(parsedCar);
                        if (stockCar == null)
                        {
                            parsedCar.Status = ParsedCarStatus.CantGetStockCar;
                            Repository.SaveChanges();
                            Console.WriteLine("Не удалось распарсить");
                            continue;
                        }

                        var advertCar = Repository.GetAdvertCar(parsedCar.Id);
                        if (advertCar == null)
                        {
                            var stockNumber = GetValue(parsedCar.FieldValues, FiledNameConstant.StockNumber);
                            var car = Repository.GetCar(stockCar.Id, dealer.Id, stockNumber);
                            //TODO: Не проверенная ветка
                            if (car != null)
                            {
                                advertCar = new AdvertCar
                                {
                                    MainAdvertCarId = car.Id,
                                    ParsedCarId = parsedCar.Id,
                                    Url = parsedCar.Url,
                                    AdvertCarPrices = new List<AdvertCarPrice>
                                    {
                                        new AdvertCarPrice {Value = priceValue, DateTime = now}
                                    }
                                };
                                Repository.CreateAdvertCar(advertCar);
                            }
                            else
                            {
                                car = new Car
                                {
                                    StockCar = stockCar,
                                    Dealer = dealer,
                                    Price = priceValue,
                                    Url = parsedCar.Url,
                                    StockNumber = stockNumber,
                                    MainAdvertCar = new MainAdvertCar
                                    {
                                        AdvertCars = new List<AdvertCar>
                                        {
                                            new AdvertCar
                                            {
                                                ParsedCarId = parsedCar.Id,
                                                Url = parsedCar.Url,
                                                AdvertCarPrices = new List<AdvertCarPrice>
                                                {
                                                    new AdvertCarPrice {Value = priceValue, DateTime = now}
                                                }
                                            }
                                        }
                                    }
                                };
                                Repository.CreateCar(car);
                            }
                            parsedCar.Status = ParsedCarStatus.AnalyzeComplete;
                            Repository.SaveChanges();
                            result.Add(new ImageDownloadCommand { Id = car.Id, Url = GetValue(parsedCar.FieldValues, FiledNameConstant.ImgPath) });
                        }
                        else
                        {
                            Repository.AddAdvertCarPrice(advertCar.Id, priceValue, now);
                            parsedCar.Status = ParsedCarStatus.AnalyzeComplete;
                            Repository.SaveChanges();
                        }
                    }

                    //после обработки порции данных её необходимо выгрузить из контекста и освободить память.
                    Repository.DetachParsedCars(parsedCars);
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
            var nameFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Name).ToLower();

            var makeFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Make) ?? GetMakeValue(nameFieldValue);
            var modelFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Model) ?? GetModelValue(nameFieldValue);
            var yearFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Year) ?? GetYearValue(nameFieldValue);
            var bodyTypeFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.BodyType);
            var styleTrimFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.StyleTrim);
            var drivetrainFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Drivetrain);

            Make make = null;
            if (makeFieldValue != null)
                make = GetDictionaryOrCreateEntity<Make>(makeFieldValue);
            Model model = null;
            if (modelFieldValue != null)
                model = GetDictionaryOrCreateEntity<Model>(modelFieldValue);
            Year year = null;
            if (yearFieldValue != null)
                year = GetDictionaryOrCreateEntity<Year>(yearFieldValue);
            BodyType bodyType = null;
            if (bodyTypeFieldValue != null)
                bodyType = GetDictionaryOrCreateEntity<BodyType>(bodyTypeFieldValue);
            StyleTrim styleTrim = null;
            if (styleTrimFieldValue != null)
                styleTrim = GetDictionaryOrCreateEntity<StyleTrim>(styleTrimFieldValue);
            Drivetrain drivetrain = null;
            if (drivetrainFieldValue != null)
                drivetrain = GetDictionaryOrCreateEntity<Drivetrain>(drivetrainFieldValue);

            if (makeFieldValue == null ||
                modelFieldValue == null ||
                yearFieldValue == null ||
                bodyTypeFieldValue == null ||
                styleTrimFieldValue == null ||
                drivetrainFieldValue == null)
                return null;

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
                if (name.Contains(year.Value.ToLower()))
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
                if (name.Contains(model.Value.ToLower()))
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
                if (name.Contains(make.Value.ToLower()))
                {
                    return make.Value;
                }
            }
            return null;
        }


    }
}
