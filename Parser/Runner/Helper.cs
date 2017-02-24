using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using DataAccess;
using DataAccess.Models;
using Utility;

namespace Runner
{
    public static class Helper
    {
        /// <summary>
        /// изменение времени в ценах, до этого все были одной даты.
        /// </summary>
        /// <param name="context"></param>
        public static void DivideDate(CarnagyContext context)
        {
            var parsedCars = context.ParsedCars.Include(a => a.Prices).ToList();
            var times = context.ParsedCars.GroupBy(a => a.CreatedTime).Select(a => a.Key).ToList();

            foreach (var parsedCar in parsedCars)
            {
                var times2 = times.Where(a => a > parsedCar.CreatedTime).ToList();
                var parsedCarPrices = parsedCar.Prices.ToList();
                for (var i = 0; i < times2.Count && i < parsedCarPrices.Count - 1; i++)
                {
                    parsedCarPrices[i + 1].DateTime = times2[i];
                }
            }
            context.SaveChanges();
        }

        /// <summary>
        /// удаляем дубли
        /// </summary>
        /// <param name="context"></param>
        public static void DeleteDublicate(CarnagyContext context)
        {
            var parsedCars = context.ParsedCars.Include(a => a.Prices).ToList();
            var abc = parsedCars.ToList().GroupBy(a => a.ForCompare).Where(a => a.Count() > 1);
            foreach (var VARIABLE in abc)
            {
                var first = VARIABLE.First();
                var other = VARIABLE.Where(a => a.Id != first.Id);
                context.ParsedCars.RemoveRange(other);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// заполняем forcomapre и фиксим ссылки
        /// </summary>
        /// <param name="context"></param>
        public static void Fillforcomapre(CarnagyContext context)
        {
            var test = context.ParsedCars.Include(a => a.Prices).ToList();
            foreach (var parsedCar in test)
            {
                parsedCar.Url = parsedCar.Url.Replace("wwwa.", "www.");
                parsedCar.ForCompare = parsedCar.Url.Split('/')[8].Trim();
            }
            context.SaveChanges();
        }

        /// <summary>
        /// чистим базу
        /// </summary>
        /// <param name="context"></param>
        public static void ClearBdFull(CarnagyContext context)
        {
            context.AdvertCars.RemoveRange(context.AdvertCars);
            context.MainAdvertCars.RemoveRange(context.MainAdvertCars);
            context.Cars.RemoveRange(context.Cars);
            context.StockCarPrices.RemoveRange(context.StockCarPrices);//??
            context.StockCars.RemoveRange(context.StockCars);
            context.Dealers.RemoveRange(context.Dealers.Where(a => a.Id != 1));
            context.Years.RemoveRange(context.Years);
            context.StyleTrims.RemoveRange(context.StyleTrims);
            context.Drivetrains.RemoveRange(context.Drivetrains);
            context.Makes.RemoveRange(context.Makes);
            context.Models.RemoveRange(context.Models);
            context.BodyTypes.RemoveRange(context.BodyTypes);
            context.SaveChanges();
        }

        /// <summary>
        /// чистим базу кроме диллера номер 1 addison
        /// </summary>
        /// <param name="context"></param>
        public static void ClearBdExeptDeleareNumber1(CarnagyContext context)
        {
            context.AdvertCars.RemoveRange(context.AdvertCars.Where(a => a.MainAdvertCar.Car.DealerId != 1));
            context.MainAdvertCars.RemoveRange(context.MainAdvertCars.Where(a => a.Car.DealerId != 1));
            context.Cars.RemoveRange(context.Cars.Where(a => a.DealerId != 1));
            context.StockCarPrices.RemoveRange(context.StockCarPrices);//??
            context.StockCars.RemoveRange(context.StockCars.Where(a => a.Cars.All(b => b.DealerId != 1)));
            context.Dealers.RemoveRange(context.Dealers.Where(a => a.Id != 1));
            context.Years.RemoveRange(context.Years.Where(a => !a.StockCars.Any()));
            context.StyleTrims.RemoveRange(context.StyleTrims.Where(a => !a.StockCars.Any()));
            context.Drivetrains.RemoveRange(context.Drivetrains.Where(a => !a.StockCars.Any()));
            context.Makes.RemoveRange(context.Makes.Where(a => !a.StockCars.Any()));
            context.Models.RemoveRange(context.Models.Where(a => !a.StockCars.Any()));
            context.SaveChanges();
        }

        /// <summary>
        /// В carnag2 нарушен порядок следования в словарях индексам соотвествует не те машины
        /// </summary>
        /// <param name="carnag2"></param>
        public static void FixCarnagy2Dicitonary(CarnagyContext carnag2)
        {
            var dic = new Dictionary<int, string> {
            {2, "Hatchback"},
            {5, "Sedan"},
            {6, "SUV"},
            {7, "Truck"},
            {11, "Convertible"},
            {12, "2dr Car"},
            {13, "Extended Cab Pickup"},
            {14, "Regular Cab Pickup"},
            {15, "Crew Cab Pickup"},
            {16, "Full - size Cargo Van"},
            {17, "Extended Cab Pickup - Standard Bed"},
            {18, "Crew Cab Pickup - Short Bed"},
            {19, "Crew Cab Pickup - Standard Bed"},
            {20, "Mini - van, Passenger"},
            {21, "Extended Cargo Van"},
            {22, "Wagon 4 Dr."},
            {23, "Crew Pickup"},
            {24, "Sedan 4 Dr."},
            {25, null},
            {26, "Cargo Van"},
            };

            foreach (var VARIABLE in dic)
            {
                var abc = carnag2.BodyTypes.SingleOrDefault(a => a.Id == VARIABLE.Key);
                abc.Value = VARIABLE.Value;
            }
            carnag2.SaveChanges();

            var dic2 = new Dictionary<int, string>
                        {
            {4 ,"Enclave                            "},
            {5 ,"Encore                             "},
            {6 ,"Envision                           "},
            {7 ,"Gran Sport                         "},
            {10 ,"LeSabre                           "},
            {16 ,"Rendezvous                        "},
            {23 ,"Verano                            "},
            {24 ,"Wildcat                           "},
            {48 ,"Camaro                            "},
            {59 ,"Colorado                          "},
            {62 ,"Corvette                          "},
            {63 ,"Cruze                             "},
            {67 ,"Equinox                           "},
            {69 ,"Impala                            "},
            {71 ,"Malibu                            "},
            {81 ,"Orlando                           "},
            {85 ,"Silverado 1500                    "},
            {88 ,"SILVERADO 3500HD                  "},
            {89 ,"Sonic                             "},
            {90 ,"Spark                             "},
            {98 ,"Suburban                          "},
            {100 ,"Tahoe                            "},
            {104 ,"Traverse                         "},
            {105 ,"Trax                             "},
            {120 ,"Acadia                           "},
            {123 ,"Canyon                           "},
            {134 ,"Sierra 1500                      "},
            {144 ,"Terrain                          "},
            {148 ,"Yukon                            "},
            {150 ,"Yukon XL                         "},
            {154 ,"Silverado 2500HD                 "},
            {155 ,"Express Cargo Van                "},
            {156 ,"Sierra 2500HD                    "},
            {157 ,"Savana Cargo Van                 "},
            {158 ,"Volt                             "},
            {159 ,"Santa Fe                         "},
            {160 ,"Focus                            "},
            {161 ,"Elantra                          "},
            {162 ,"Sonata                           "},
            {163 ,"Grand Caravan                    "},
            {164 ,"Fusion                           "},
            {165 ,"Corolla                          "},
            {166 ,"Pathfinder                       "},
            {167 ,"RAV4                             "},
            {168 ,"Grand Cherokee                   "},
            {169 ,"CX-5                             "},
            {170 ,"XV Crosstrek                     "},
            {171 ,"Durango                          "},
            {172 ,"ATS                              "},
            {173 ,"C-Class                          "},
            {174 ,"1500                             "},
            {175 ,"RX 350                           "},
            {176 ,"X5                               "},
            {177 ,"Charger                          "},
            {178 ,"XF                               "},
            {179 ,"4Runner                          "},
            {180 ,"F-150                            "},
            {181 ,"GL-Class                         "},
            {182 ,"Journey                          "},
            {183 ,"Jetta Sedan                      "},
            {184 ,"Passat                           "},
            {185 ,"Forte                            "},
            {186 ,"Sorento                          "},
            {187 ,"Golf                             "},
            {188 ,"Camry                            "},
            {189 ,"Mustang                          "},
            {190 ,"Expedition Max                   "},
            {191 ,"1 Series                         "},
            {192 ,"CTS Sedan                        "},
            {193 ,"Pilot                            "},
            {194 ,"300                              "},
            {195 ,"Wrangler                         "},
            {196 ,"Q50                              "},
            {197 ,"Outback                          "},
            {198 ,"S5                               "},
            {199 ,"X6 M                             "},
            {200 ,"Escalade                         "} ,

                        };

            foreach (var VARIABLE in dic2)
            {
                var abc = carnag2.Models.SingleOrDefault(a => a.Id == VARIABLE.Key);
                abc.Value = VARIABLE.Value.Trim();
            }
            carnag2.SaveChanges();
        }

        /// <summary>
        /// миграция из carnag2 в carnag5
        /// </summary>
        /// <param name="carnag2"></param>
        /// <param name="carnagy5"></param>
        public static void MigrateFromOlddatabaseToNew(CarnagyContext carnag2, CarnagyContext carnagy5)
        {
            var cars = carnag2.Cars.Where(c => c.DealerId == 1).ToList();
            foreach (var car in cars)
            {
                if (
                    (car.Id == 533) ||
                        (car.Id == 3137) ||
                        (car.Id == 3046))
                    continue;
                var singleOrDefault = car.MainAdvertCar
                    .AdvertCars
                    .SingleOrDefault(a => a.IsDealer);
                if (singleOrDefault == null)
                    continue;

                var styleTrims = carnagy5.StyleTrims.ToList();
                var bodyTypes = carnagy5.BodyTypes.ToList();
                var drivetrains = carnagy5.Drivetrains.ToList();

                var make = carnagy5.Makes.SingleOrDefault(a => a.Value == car.StockCar.Make.Value.Trim()) ?? new Make { Value = car.StockCar.Make.Value.Trim() };
                var model = carnagy5.Models.SingleOrDefault(a => a.Value == car.StockCar.Model.Value.Trim()) ?? new Model { Value = car.StockCar.Model.Value.Trim() };
                var year = carnagy5.Years.SingleOrDefault(a => a.Value == car.StockCar.Year.Value.Trim()) ?? new Year { Value = car.StockCar.Year.Value.Trim() };

                var bodyType = carnagy5.BodyTypes.SingleOrDefault(a => a.Value == (car.StockCar.BodyType.Value ?? "Not know").Trim()) ??
                    (bodyTypes.Where(a => car.StockCar.BodyType.Value.Trim().Contains(a.Value)).OrderByDescending(a => a.Value.Length).FirstOrDefault() ??
                    new BodyType { Value = car.StockCar.BodyType.Value.Trim() });

                var styleTrim = carnagy5.StyleTrims.SingleOrDefault(a => a.Value == (car.StockCar.StyleTrim.Value ?? "Not know").Trim()) ??
                                (styleTrims.Where(a => car.StockCar.StyleTrim.Value.Trim().Contains(a.Value)).OrderByDescending(a => a.Value.Length).FirstOrDefault() ??
                                new StyleTrim { Value = car.StockCar.StyleTrim.Value.Trim() });


                var drivetrain = carnagy5.Drivetrains.SingleOrDefault(a => a.Value == (car.StockCar.Drivetrain.Value ?? "Not know").Trim()) ??
                    (drivetrains.Where(a => car.StockCar.Drivetrain.Value.Trim().Contains(a.Value)).OrderByDescending(a => a.Value.Length).FirstOrDefault() ??
                    new Drivetrain { Value = car.StockCar.Drivetrain.Value.Trim() });

                //carnagy5.Makes.AddOrUpdate(a => a.Id, make);
                //carnagy5.Models.AddOrUpdate(a => a.Id, model);
                //carnagy5.Years.AddOrUpdate(a => a.Id, year);
                //carnagy5.BodyTypes.AddOrUpdate(a => a.Id, bodyType);
                //carnagy5.StyleTrims.AddOrUpdate(a => a.Id, styleTrim);
                //carnagy5.Drivetrains.AddOrUpdate(a => a.Id, drivetrain);
                //carnagy5.SaveChanges();


                var stockCar = carnagy5.StockCars.SingleOrDefault(a => a.MakeId == make.Id &&
                                                             a.ModelId == model.Id &&
                                                             a.YearId == year.Id &&
                                                             a.BodyTypeId == bodyType.Id &&
                                                             a.StyleTrimId == styleTrim.Id &&
                                                             a.DrivetrainId == drivetrain.Id) ?? new StockCar
                                                             {
                                                                 //MakeId = make.Id,
                                                                 //ModelId = model.Id,
                                                                 //YearId = year.Id,
                                                                 //BodyTypeId = bodyType.Id,
                                                                 //DrivetrainId = drivetrain.Id,
                                                                 //StyleTrimId = styleTrim.Id,
                                                                 Make = make,
                                                                 Model = model,
                                                                 Year = year,
                                                                 BodyType = bodyType,
                                                                 Drivetrain = drivetrain,
                                                                 StyleTrim = styleTrim
                                                             };
                carnagy5.StockCars.AddOrUpdate(stockCar);

                var list = singleOrDefault
                                    .AdvertCarPrices
                                    .Select(a => new AdvertCarPrice { Value = a.Value, DateTime = a.DateTime })
                                    .ToList();
                carnagy5.Cars.Add(new Car
                {
                    DealerId = 1,
                    StockCar = stockCar,
                    Price = list.Last().Value,
                    StockNumber = car.StockNumber,
                    Url = car.Url,
                    MainAdvertCar = new MainAdvertCar
                    {
                        IsDeleted = car.MainAdvertCar.IsDeleted,
                        AdvertCars = new List<AdvertCar>
                        {
                            new AdvertCar
                            {
                                IsDealer = true,
                                Url = singleOrDefault.Url,
                                AdvertCarPrices = list
                            }
                        }
                    }
                });
                carnagy5.SaveChanges();
            }
        }

        /// <summary>
        /// перенос цен
        /// </summary>
        /// <param name="carnag2"></param>
        /// <param name="carnagy5"></param>
        public static void MovePrice(CarnagyContext carnag2, CarnagyContext carnagy5)
        {
            var abcd = carnagy5.Cars.Select(a => a.StockNumber).ToList();
            var bcdfsf = carnag2.Cars.Where(a => a.Id != 533 || a.Id != 3125 || a.Id != 3137 || a.Id != 3046).Where(a => abcd.Contains(a.StockNumber)).ToList();
            var bcdfsf2 = carnag2.Cars.Where(a => a.Id != 533 || a.Id != 3125 || a.Id != 3137 || a.Id != 3046).Where(a => a.DealerId == 1).Where(a => !abcd.Contains(a.StockNumber)).ToList();
            foreach (var source in bcdfsf) //carnag2.Cars.Where(a => a.DealerId == 1)
            {
                if (
                    (source.Id == 533) ||
                        (source.Id == 3125) ||
                        (source.Id == 3137) ||
                        (source.Id == 3046))
                    continue;
                var singleOrDefault = source.MainAdvertCar
                    .AdvertCars
                    .SingleOrDefault(a => a.IsDealer);
                if (singleOrDefault == null)
                    continue;

                var car = carnagy5.Cars.SingleOrDefault(a => a.StockNumber == source.StockNumber);
                if (car != null)
                {
                    var abc = car.MainAdvertCar.AdvertCars.First();
                    var abc2 = abc.AdvertCarPrices.First();
                    var list =
                        singleOrDefault.AdvertCarPrices.Select(
                            a => new AdvertCarPrice { Value = a.Value, DateTime = a.DateTime, AdvertCarId = abc.Id }).ToList();
                    list.Add(new AdvertCarPrice { Value = abc2.Value, DateTime = abc2.DateTime, AdvertCarId = abc.Id });
                    carnagy5.AdvertCarPrices.Remove(abc2);
                    carnagy5.AdvertCarPrices.AddRange(list);
                }
                else
                {
                    //carnagy5.Cars.Add(new Car
                    //{
                    //    DealerId = 1,
                    //    StockCar = stockCar,
                    //    Price = list.Last().Value,
                    //    StockNumber = car.StockNumber,
                    //    Url = car.Url,
                    //    MainAdvertCar = new MainAdvertCar
                    //    {
                    //        IsDeleted = car.MainAdvertCar.IsDeleted,
                    //        AdvertCars = new List<AdvertCar>
                    //    {
                    //        new AdvertCar
                    //        {
                    //            IsDealer = true,
                    //            Url = singleOrDefault.Url,
                    //            AdvertCarPrices = list
                    //        }
                    //    }
                    //    }
                    //});

                }
            }
            carnagy5.SaveChanges();
        }

        /// <summary>
        /// Формирирование цен для stockCar за все промежутки времени
        /// </summary>
        /// <param name="context"></param>
        public static void AddAllStockCarPrices(CarnagyContext context)
        {
            var stockCars = context.Set<StockCar>()
                .Include(a => a.Cars.Select(b => b.MainAdvertCar.AdvertCars.Select(c => c.AdvertCarPrices)))
                .ToList();

            var timeStart = DateTime.Now;
            foreach (var stockCar in stockCars)
            {
                if (!stockCar.Cars.Any()) continue;

                var list = new List<AdvertCarPrice>();
                foreach (var car in stockCar.Cars)
                {

                    var advertsCar = car.MainAdvertCar.AdvertCars.SingleOrDefault(a => a.IsDealer) ??
                                      car.MainAdvertCar.AdvertCars.FirstOrDefault(a => !a.IsDealer);
                    list.AddRange(advertsCar.AdvertCarPrices);
                }

                var group = list.GroupBy(a => a.DateTime.ToShortDateString());

                foreach (var VARIABLE in group)
                {
                    var averagePrice = (int)VARIABLE.Average(a => a.Value);
                    stockCar.StockCarPrices.Add(new StockCarPrice
                    {
                        DateTime = DateTime.Parse(VARIABLE.Key),
                        Value = averagePrice
                    });
                }

                stockCar.Price =
                    stockCar.StockCarPrices.OrderByDescending(a => a.DateTime).First().Value;
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Востанавливаем данные в таблице AdvertCarPrice за 24.12.2016
        /// </summary>
        /// <param name="context"></param>
        public static void RestoreAdvertCarPriceFromDate(CarnagyContext context)
        {
            var retoreDate = DateTime.Parse("24/12/2016");
            var price = context.Prices.Include(a => a.ParsedCar.AdvertCars).ToList().Where(a => a.DateTime.Date == retoreDate);
            foreach (var price1 in price)
            {
                var AdvertCar = price1.ParsedCar.AdvertCars.First();
                var priceValue = 0.0;
                double.TryParse(
                    price1.Value,
                    NumberStyles.AllowCurrencySymbol |
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowThousands,
                    new CultureInfo("en-US"),
                    out priceValue);

                AdvertCar.AdvertCarPrices.Add(new AdvertCarPrice { Value = priceValue, DateTime = retoreDate });
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Скачать изображения для машин
        /// </summary>
        /// <param name="context"></param>
        /// <param name="downloadImage"></param>
        public static void DownloadImage(CarnagyContext context, IDownloadImage downloadImage)
        {
            var imagesCommands = context.Cars.Include(a => a.MainAdvertCar).Where(a => a.DealerId == 1 && !a.MainAdvertCar.IsDeleted)
                //.Take(100)
                .ToList()
                .Select(a => new ImageDownloadCommand
                {
                    Id = a.Id,
                    Url = a.ImageSrc
                });
            downloadImage.Download("Cars", imagesCommands);
        }

        /// <summary>
        /// Откатываем дату на несколько часов назад
        /// </summary>
        /// <param name="context"></param>
        public static void MoveAdvertCarPriceDate(CarnagyContext context)
        {
            var date = DateTime.Parse("01/01/2017");
            var advertCars = context.AdvertCarPrices.Where(a => a.DateTime > date).ToList();
            foreach (var advertCarPrice in advertCars)
            {
                advertCarPrice.DateTime = advertCarPrice.DateTime.AddHours(-6);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Указываем дату создания машины и дату удаления
        /// </summary>
        /// <param name="context"></param>
        public static void AddCreatedTime(CarnagyContext context)
        {
            var cars = context.Cars.Include(a => a.MainAdvertCar.AdvertCars.Select(b => b.AdvertCarPrices)).ToList();
            foreach (var car in cars)
            {
                var advertCarPrices = car.MainAdvertCar.AdvertCars.SelectMany(a => a.AdvertCarPrices).OrderBy(a => a.DateTime);
                car.CreatedTime = advertCarPrices.First().DateTime;
                var lastTime = advertCarPrices.Last().DateTime;
                if (car.MainAdvertCar.IsDeleted)
                    car.DeletedTime = lastTime;
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Удаление лишних цен в AdvertCarPrice
        /// </summary>
        /// <param name="context"></param>
        public static void DeleteAdditionalAdvertsCarPrice(CarnagyContext context)
        {
            var parssedCars = context.ParsedCars.Include(a => a.Prices).ToList();
            foreach (var parssedCar in parssedCars)
            {
                var advertCar = parssedCar.AdvertCars.FirstOrDefault();
                if (advertCar == null)
                    continue;
                var lastDate = parssedCar.Prices.OrderBy(a => a.DateTime).Last().DateTime.Date;

                var deletedAdvertCarPrice = advertCar.AdvertCarPrices
                    .ToList()
                    .Where(a => a.DateTime.Date > lastDate)
                    .OrderBy(a => a.DateTime);
                context.AdvertCarPrices.RemoveRange(deletedAdvertCarPrice);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Исправление коротких названий провинций на длинные
        /// </summary>
        public static void ChangeDealerProvinceFromShortToFull(CarnagyContext context)
        {
            var dic = new Dictionary<string, string>
            {
                  { "ON", "Ontario" },
                  { "QC", "Quebec" },
                  { "NS", "Nova Scotia" },
                  { "NB", "New Brunswick" },
                  { "MB", "Manitoba" },
                  { "BC", "British Columbia" },
                  { "PE", "Prince Edward Island" },
                  { "SK", "Saskatchewan" },
                  { "AB", "Alberta" },
                  { "NL", "Newfoundland and Labrador" }
            };
            var dealers = context.Dealers.ToList();
            foreach (var dealer in dealers)
            {
                if (dic.ContainsKey(dealer.Province) == false) continue;

                dealer.Province = dic[dealer.Province];
            }

            context.SaveChanges();
        }
    }
}
