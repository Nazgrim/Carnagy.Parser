using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataAccess.Models;
using DataAccess.Repositories;
using Newtonsoft.Json;
using Utility;

namespace AddisongmParseAndAnalyze
{
    public class ParseAndAnalyze : IParseAndAnalyze
    {
        private const string InventoryUrl = "http://addisongm.com/data/inventory.json";
        private const string HttpWwwAddisongmCom = "http://www.addisongm.com";
        private IAnalyseRepository Repository { get; set; }
        private IDownloadImage DownloadImage { get; set; }
        private IDownloadManager DownloadManager { get; set; }

        public ParseAndAnalyze(IAnalyseRepository repository, IDownloadImage downloadImage, IDownloadManager downloadManager)
        {
            Repository = repository;
            DownloadImage = downloadImage;
            DownloadManager = downloadManager;
        }

        public void Run()
        {
            var rooobject = GetRootobject();
            var dealer = Repository.GetDealerByWebSireUrl(HttpWwwAddisongmCom);
            if (dealer == null)
                return;

            var vehicles = rooobject.vehicles.ToList();
            var stockNumbers = Repository.GetAllStockNumber(dealer.Id);
            FillDatabase(rooobject.vehicles.ToList(), dealer, stockNumbers);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //var images = Repository.GetCarsByDealerId(dealer.Id)
            //    .Select(a => new ImageDownloadCommand
            //    {
            //        Id = a.Id,
            //        Url = vehicles.FirstOrDefault(b => b.stocknumber == a.StockNumber)?.picture
            //    })
            //    .Where(a => !string.IsNullOrWhiteSpace(a.Url));
            //DownloadImage.Download("Cars", images);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }

        private Rootobject GetRootobject()
        {
            //var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            //var path = System.IO.Path.Combine(sourceDirectory, "json\\inventory.json");
            //var json = System.IO.File.ReadAllText(path);
            var json = DownloadManager.DownloadString(InventoryUrl);
            var result = JsonConvert.DeserializeObject<Rootobject>(json);
            return result;
        }

        private void FillDatabase(List<Vehicle> vehicles, Dealer dealer, List<Car> stockNumbers)
        {
            var now = DateTime.Now;
            foreach (var vehicle in vehicles)
            {
                var carUrl = $"http://addisongm.com/view/{vehicle.condition}-{vehicle.year}-{vehicle.make}-{vehicle.model}-{vehicle.id}";
                carUrl = System.Text.RegularExpressions.Regex.Replace(carUrl, @"\s g", "-").ToLower();
                var car = stockNumbers.SingleOrDefault(a => a.StockNumber == vehicle.stocknumber);
                if (car == null)
                {
                    car = AddCar(vehicle, dealer, carUrl);
                }
                else
                {
                    car.Price = vehicle.price;
                    car.Url = carUrl;
                    car.ImageSrc = vehicle.picture;
                    stockNumbers.Remove(car);
                }
                var advertCar = GetOrCreateAdvertCar(car);

                Repository.AddAdvertCarPrice(advertCar.Id, vehicle.price, now);
            }

            Repository.DeleteCars(stockNumbers);

            Repository.SaveChanges();
        }

        private Car AddCar(Vehicle vehicle, Dealer dealer, string carUrl)
        {
            if (vehicle.bodystyle == "Sport Utility")
            {
                vehicle.bodystyle = "SUV";
            }

            if (vehicle.bodystyle == "4dr Car")
            {
                vehicle.bodystyle = "Sedan";
            }

            var make = GetDictionaryOrCreateEntity<Make>(vehicle.make);
            var model = GetDictionaryOrCreateEntity<Model>(vehicle.model);
            var year = GetDictionaryOrCreateEntity<Year>(vehicle.year);
            var bodyType = GetDictionaryOrCreateEntity<BodyType>(vehicle.bodystyle);
            var styleTrim = GetDictionaryOrCreateEntity<StyleTrim>(vehicle.trim);
            var drivetrain = GetDictionaryOrCreateEntity<Drivetrain>(vehicle.drivetrain);

            var stockCar = GetOrCreateStocCar(make, model, year, bodyType, styleTrim, drivetrain, vehicle.msrp);

            var car = new Car
            {
                DealerId = dealer.Id,
                StockCarId = stockCar.Id,
                StockNumber = vehicle.stocknumber,
                Price = vehicle.price,
                Url = carUrl,
                ImageSrc = vehicle.picture
            };
            Repository.CreateCar(car);

            return car;
        }

        private AdvertCar GetOrCreateAdvertCar(Car car)
        {
            var advertCars = Repository.GetAdvertCars(car.Id);
            if (!advertCars.Any())
            {
                var advertCar = new AdvertCar
                {
                    Url = car.Url,
                    MainAdvertCar = new MainAdvertCar { Car = car },
                    IsDealer = true,
                    ImageSrc = car.ImageSrc
                };
                Repository.CreateAdvertCar(advertCar);
                return advertCar;
            }

            var mainAdvertCar = advertCars.FirstOrDefault(a => a.IsDealer);
            if (mainAdvertCar != null) return mainAdvertCar;

            mainAdvertCar = new AdvertCar
            {
                Url = car.Url,
                MainAdvertCarId = car.Id,
                IsDealer = true,
                ImageSrc = car.ImageSrc
            };
            Repository.CreateAdvertCar(mainAdvertCar);
            return mainAdvertCar;
        }

        private StockCar GetOrCreateStocCar(Make make, Model model, Year year, BodyType bodyType, StyleTrim styleTrim, Drivetrain drivetrain, float msrp)
        {
            Func<StockCar, bool> filter = a => a.YearId == year.Id &&
                                                    a.MakeId == make.Id &&
                                                    a.ModelId == model.Id &&
                                                    a.BodyTypeId == bodyType.Id &&
                                                    a.StyleTrimId == styleTrim.Id &&
                                                    a.DrivetrainId == drivetrain.Id;

            var stockCar = Repository.GetStockCar(filter);
            if (stockCar != null)
                return stockCar;

            stockCar = new StockCar
            {
                Year = year,
                Make = make,
                Model = model,
                BodyType = bodyType,
                Drivetrain = drivetrain,
                StyleTrim = styleTrim,
                MsrpPrice = msrp
            };
            Repository.CreateStockCar(stockCar);

            return stockCar;
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
    }
}
