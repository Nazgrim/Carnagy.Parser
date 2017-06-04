using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IAnalyseRepository Repository;
        private IDownloadImage DownloadImager;

        private const int ParrsedCarByTimes = 500;
        private const string DefaultValue = "Not know";
        private DateTime Current;

        public AnalyzerBase(IAnalyseRepository repository, IDownloadImage downloadImager)
        {
            Repository = repository;
            DownloadImager = downloadImager;
        }

        public void Run()
        {
            Analyze();
        }

        private void Analyze()
        {
            Current = DateTime.Now;
            var images = new List<ImageDownloadCommand>();
            var configuration = Repository.GetConfigurations();
            //AnalyzeDealerConfiguration(configuration);
            var autotraderImages = AnalyzeAutotraderConfiguration(configuration);
            images.AddRange(autotraderImages);
            //DownloadImager.Download("Cars", images.Where(a => !string.IsNullOrWhiteSpace(a.Url)));
        }

        private List<ImageDownloadCommand> AnalyzeAutotraderConfiguration(List<MainConfiguration> configuration)
        {
            var _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var result = new List<ImageDownloadCommand>();
            var autotraderConfiguration = configuration.Where(a => !a.DealerId.HasValue);
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
                    page++;
                    //если данных нет выходим из цикла
                    if (!parsedCars.Any())
                    {
                        break;
                    }
                    WriteToLog($"Количество parsedCars забранных из базы {parsedCars.Count}");

                    var deletedParsedCar = new List<ParsedCar>();
                    var updateParsedCar = new List<ParsedCar>();
                    var newParsedCar = new List<ParsedCar>();
                    foreach (var parsedCar in parsedCars)
                    {
                        if (parsedCar.IsDeleted)
                        {
                            deletedParsedCar.Add(parsedCar);
                        }
                        else if (parsedCar.AdvertCars.Any())
                        {
                            updateParsedCar.Add(parsedCar);
                        }
                        else
                        {
                            newParsedCar.Add(parsedCar);
                        }
                    }

                    if (deletedParsedCar.Any())
                    {
                        WriteToLog($"Количество deletedParsedCar забранных из базы {deletedParsedCar.Count}");
                        foreach (var parsedCar in deletedParsedCar)
                        {
                            var advertCar = parsedCar.AdvertCars.FirstOrDefault ();
                            if (advertCar!=null && advertCar.MainAdvertCar.IsDeleted == false)
                            {
                                advertCar.MainAdvertCar.IsDeleted = true;
                                advertCar.MainAdvertCar.Car.DeletedTime = Current;
                            }
                            parsedCar.IsCarDeleted = true;
                        }
                        Repository.SaveChanges();
                        WriteToLog($"Обработка deletedParsedCar завершена за {GetTime(_stopwatch.Elapsed)}");
                    }

                    if (updateParsedCar.Any())
                    {
                        WriteToLog($"Количество updateParsedCar забранных из базы {updateParsedCar.Count}");
                        Repository.StopDetecChanges();
                        foreach (var parsedCar in updateParsedCar)
                        {
                            var advertCar = parsedCar.AdvertCars.First();// 1 to 1
                            var price = parsedCar.Prices
                                .OrderByDescending(a => a.DateTime)
                                .FirstOrDefault();

                            //TODO: выбрасывать эти машины не стоит, но нужно учитывать что они могут нарушить логику работы расчетов
                            var priceValue = 0.0;
                            if (!double.TryParse(
                                price?.Value ?? string.Empty,
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

                            Repository.AddAdvertCarPrice(advertCar.Id, priceValue, price.DateTime);
                            if (parsedCar.Status != ParsedCarStatus.AnalyzeComplete)
                                parsedCar.Status = ParsedCarStatus.AnalyzeComplete;
                        }
                        Repository.StartDetecChanges();
                        Repository.SaveChanges();
                        WriteToLog($"Обработка updateParsedCar завершена за {GetTime(_stopwatch.Elapsed)}");
                    }

                    if (newParsedCar.Any())
                    {
                        WriteToLog($"Количество newParsedCar забранных из базы {newParsedCar.Count}");
                        foreach (var parsedCar in newParsedCar)
                        {
                            var listPrice = new List<AdvertCarPrice>();
                            var allIsGood = true;
                            foreach (var price in parsedCar.Prices)
                            {
                                //TODO: выбрасывать эти машины не стоит, но нужно учитывать что они могут нарушить логику работы расчетов
                                var priceValue = 0.0;
                                if (!double.TryParse(
                                    price.Value,
                                    NumberStyles.AllowCurrencySymbol |
                                    NumberStyles.AllowDecimalPoint |
                                    NumberStyles.AllowThousands,
                                    new CultureInfo("en-US"),
                                    out priceValue))
                                {
                                    parsedCar.Status = ParsedCarStatus.CannotParsePrice;
                                    Repository.SaveChanges();
                                    allIsGood = false;
                                    break;
                                }
                                listPrice.Add(new AdvertCarPrice { Value = priceValue, DateTime = price.DateTime });
                            }

                            if (!allIsGood)
                                continue;
                            var msrpValue = GetValue(parsedCar.FieldValues, FiledNameConstant.MSRP);
                            var msrp = 0.0;
                            if (!double.TryParse(
                                    msrpValue,
                                    NumberStyles.AllowCurrencySymbol |
                                    NumberStyles.AllowDecimalPoint |
                                    NumberStyles.AllowThousands,
                                    new CultureInfo("en-US"),
                                    out msrp))
                            {
                                msrp = listPrice.OrderByDescending(a => a.DateTime).First().Value;
                            }

                            //без диллера некуда прикрепить машину.
                            var dealerWebSite = GetValue(parsedCar.FieldValues, FiledNameConstant.DealerWebSite);
                            if (dealerWebSite == null)
                            {
                                parsedCar.Status = ParsedCarStatus.NoDealerWebSite;
                                Repository.SaveChanges();
                                continue;
                            }

                            var advertCar = new AdvertCar
                            {
                                ParsedCarId = parsedCar.Id,
                                Url = parsedCar.Url,
                                AdvertCarPrices = listPrice,
                                ImageSrc = GetImage(parsedCar)
                            };
                            var stockNumber = GetValue(parsedCar.FieldValues, FiledNameConstant.StockNumber);
                            var dealer = GetOrCreateDealer(parsedCar, dealerWebSite);
                            var stockCar = GetStockCar(parsedCar);
                            if (stockCar == null)
                            {
                                parsedCar.Status = ParsedCarStatus.CantGetStockCar;
                                Repository.SaveChanges();
                                Console.WriteLine("Не удалось распарсить");
                                continue;
                            }

                            var car = Repository.GetCarByStockNumber(stockNumber, dealer.Id);
                            if (car == null)
                            {
                                car = CreateCar(parsedCar, stockCar, dealer, listPrice.Last().Value, stockNumber, msrp);
                            }
                            else if (stockNumber != null)
                            {
                                RebalanceStockCar(car, stockCar);
                            }

                            car.ImageSrc = GetImage(parsedCar);

                            car.MainAdvertCar.AdvertCars.Add(advertCar);
                            parsedCar.Status = ParsedCarStatus.AnalyzeComplete;
                            Repository.SaveChanges();
                            result.Add(new ImageDownloadCommand
                            {
                                Id = car.Id,
                                Url = GetValue(parsedCar.FieldValues, FiledNameConstant.ImgPath)
                            });
                            Repository.SaveChanges();
                        }
                        WriteToLog($"Обработка newParsedCar завершена за {GetTime(_stopwatch.Elapsed)}");
                    }
                    //после обработки порции данных её необходимо выгрузить из контекста и освободить память.
                    //Repository.DetachParsedCars(parsedCars);
                }
            }
            _stopwatch.Stop();
            return result;
        }

        private string GetImage(ParsedCar parsedCar)
        {
            var imageSrc = GetValue(parsedCar.FieldValues, FiledNameConstant.ImgPath);
            if (string.IsNullOrWhiteSpace(imageSrc))
            {
                return string.Empty;
            }
            imageSrc = imageSrc.Split('?')[0] + "?w=160&h=120";
            return imageSrc;
        }

        private void RebalanceStockCar(Car car, StockCar newStockCar)
        {
            var oldStockCar = car.StockCar;
            var cars = oldStockCar.Cars;
            var bodyType = FixValue(oldStockCar.BodyType, newStockCar.BodyType);
            var styleTrim = FixValue(oldStockCar.StyleTrim, newStockCar.StyleTrim);
            var drivetrain = FixValue(oldStockCar.Drivetrain, newStockCar.Drivetrain);

            if (bodyType.Id == oldStockCar.BodyTypeId &&
                styleTrim.Id == oldStockCar.StyleTrimId &&
                drivetrain.Id == oldStockCar.DrivetrainId
                )
                return;
            //ищем уже сущесвующий
            Func<StockCar, bool> filter = a => a.YearId == oldStockCar.YearId
                                              && a.MakeId == oldStockCar.MakeId
                                              && a.ModelId == oldStockCar.ModelId
                                              && a.BodyTypeId == bodyType.Id
                                              && a.StyleTrimId == styleTrim.Id
                                              && a.DrivetrainId == drivetrain.Id;

            var stockCar = Repository.GetStockCar(filter);

            if (cars.Count > 1)
            {
                if (stockCar == null)
                {
                    stockCar = new StockCar
                    {
                        Make = oldStockCar.Make,
                        Year = oldStockCar.Year,
                        Model = oldStockCar.Model,
                        BodyType = bodyType,
                        Drivetrain = drivetrain,
                        StyleTrim = styleTrim,
                    };
                }
                //если stockCar==null создатся новый, в противном случаии поменяется ссылка.
                car.StockCar = stockCar;
            }
            else if (stockCar == null)
            {
                //Обновляем существующий 
                oldStockCar.BodyType = bodyType;
                oldStockCar.StyleTrim = styleTrim;
                oldStockCar.Drivetrain = drivetrain;
            }
            else
            {
                //удаляем текущей и ссылаемся на другой
                car.StockCarId = stockCar.Id;
                Repository.DeleteStockCar(oldStockCar);
            }
        }

        private T FixValue<T>(T oldDictionaryEntity, T newDictionaryEntity)
             where T : class, IDictionaryEntity
        {
            var oldDictionaryValueLower = oldDictionaryEntity.Value.ToLower();
            var newDictionaryValueLower = newDictionaryEntity.Value.ToLower();

            //значения равны, возвращаем текущий
            if (oldDictionaryValueLower == newDictionaryValueLower)
                return oldDictionaryEntity;

            //текущий не известене, возвращаем новый
            if (oldDictionaryValueLower == DefaultValue.ToLower())
                return newDictionaryEntity;

            //в новом, есть шумы, текущий более точный
            if (newDictionaryValueLower.Contains(oldDictionaryValueLower))
                return oldDictionaryEntity;

            //в текущем, есть шумы, новый более точный
            if (oldDictionaryValueLower.Contains(newDictionaryValueLower))
                return newDictionaryEntity;

            //это парсер autotrder его значения более приоритетние
            return newDictionaryEntity;
        }

        public void Сalculation()
        {
            var stockCars = Repository.GetStockCars();
            var timeStart = DateTime.Now;
            var i = 0;
            foreach (var stockCar in stockCars)
            {
                var cars = stockCar.Cars.Where(a => a.Price != 0 && !a.MainAdvertCar.IsDeleted);
                if (cars.Any() == false) continue;
                foreach (var car in cars)
                {
                    var advertsCar = car.MainAdvertCar.AdvertCars.SingleOrDefault(a => a.IsDealer) ??
                                      car.MainAdvertCar.AdvertCars.FirstOrDefault(a => !a.IsDealer);
                    var lastPrice = advertsCar.AdvertCarPrices.OrderByDescending(a => a.DateTime).First();
                    car.Price = lastPrice.Value;
                }

                var averagePrice = (int)cars
                    .Average(a => a.Price);
                stockCar.Price = averagePrice;
                stockCar.StockCarPrices.Add(new StockCarPrice
                {
                    DateTime = timeStart,
                    Value = averagePrice
                });
                if (i >= 100)
                {
                    Repository.SaveChanges();
                    i = 0;
                }
                else
                {
                    i++;
                }       
            }
            Repository.SaveChanges();
        }

        private Dealer GetOrCreateDealer(ParsedCar parssedCar, string dealerWebSite)
        {
            var dealer = Repository.GetDealerByWebSireUrl(dealerWebSite);

            if (dealer != null) return dealer;

            var dealerName = GetValue(parssedCar.FieldValues, FiledNameConstant.DealerName);
            var dealerLogo = GetValue(parssedCar.FieldValues, FiledNameConstant.DealerLogo);
            var dealerPlace = GetValue(parssedCar.FieldValues, FiledNameConstant.DealerPlace);

            dealer = new Dealer
            {
                Location = dealerPlace,
                Logo = dealerLogo,
                WebSireUrl = dealerWebSite,
                IsCreated = false,
                Name = dealerName
            };
            Repository.CreateDealer(dealer);

            return dealer;
        }

        private string GetValue(IEnumerable<FieldValue> fieldValues, string key)
        {
            var value = fieldValues.SingleOrDefault(a => a.Field.Name == key)?.Value;
            return value == "-" ? null : value;
        }

        private T GetOrCreateDictionaryEntity<T>(IEnumerable<FieldValue> fieldValues, string key, string name, bool isCreateDefault = false)
            where T : class, IDictionaryEntity, new()
        {
            //ищем значение в колекции значений
            var value = GetValue(fieldValues, key);

            var dictionaryEntities = Repository.GetDictionaryEntity<T>();

            //если нашли, ищем в базе
            T entity = GetDictionaryEntity(value, dictionaryEntities, name);

            if (entity != null) return entity;

            //не нашли в имени и значение пустое, если главные параметры возвращаем null
            //если не главные возвращаем дефолтное значение            
            if (value == null)
            {
                return isCreateDefault ? dictionaryEntities.SingleOrDefault(a => a.Value == DefaultValue) : null;
            }
            return new T { Value = value };
        }

        private StockCar GetStockCar(ParsedCar parssedCar)
        {
            var nameFieldValue = GetValue(parssedCar.FieldValues, FiledNameConstant.Name).ToLower();

            var make = GetOrCreateDictionaryEntity<Make>(parssedCar.FieldValues, FiledNameConstant.Make, nameFieldValue);
            var model = GetOrCreateDictionaryEntity<Model>(parssedCar.FieldValues, FiledNameConstant.Model, nameFieldValue);
            var year = GetOrCreateDictionaryEntity<Year>(parssedCar.FieldValues, FiledNameConstant.Year, nameFieldValue);
            var bodyType = GetOrCreateDictionaryEntity<BodyType>(parssedCar.FieldValues, FiledNameConstant.BodyType, nameFieldValue, true);
            var styleTrim = GetOrCreateDictionaryEntity<StyleTrim>(parssedCar.FieldValues, FiledNameConstant.StyleTrim, nameFieldValue, true);
            var drivetrain = GetOrCreateDictionaryEntity<Drivetrain>(parssedCar.FieldValues, FiledNameConstant.Drivetrain, nameFieldValue, true);

            if (make == null ||
                model == null ||
                year == null ||
                bodyType == null ||
                styleTrim == null ||
                drivetrain == null)
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

        private T GetDictionaryEntity<T>(string name, List<T> dictionaryEntities, string alternaty = null) where T : class, IDictionaryEntity
        {
            var listResult = new List<T>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                //ищем прямое совпадение
                name = name.ToLower();
                var entity = dictionaryEntities.SingleOrDefault(a => a.Value.ToLower() == name);
                if (entity != null) return entity;

                //ищем все совпадения в имени
                //сохраняем все результаты в список, далее сортируем его в порядке убывания, что бы наиболее
                //точное совпадение оказалось наверху(Crew Cab Pickup - Standard Bed точнее чем Crew Cab) и берем первое значение
                listResult.AddRange(dictionaryEntities.Where(a => name.Contains(a.Value.ToLower())));
            }

            if (string.IsNullOrWhiteSpace(alternaty)) return listResult.OrderByDescending(a => a.Value.Length).FirstOrDefault();

            //ищем все совпадения в альтернативном имени
            listResult.AddRange(dictionaryEntities.Where(a => alternaty.Contains(a.Value.ToLower())));
            return listResult.OrderByDescending(a => a.Value.Length).FirstOrDefault();
        }

        private Car CreateCar(ParsedCar parsedCar, StockCar stockCar, Dealer dealer, double priceValue, string stockNumber, double msrp)
        {
            var car = new Car
            {
                StockCar = stockCar,
                Dealer = dealer,
                Price = priceValue,
                Url = parsedCar.Url,
                StockNumber = stockNumber,
                MainAdvertCar = new MainAdvertCar(),
                CreatedTime = Current,
                MsrpPrice = msrp
            };

            Repository.CreateCar(car);
            return car;
        }

        private void WriteToLog(string text)
        {
            Console.WriteLine(text);
        }

        private string GetTime(TimeSpan ts)
        {
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            return elapsedTime;
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
    }
}
