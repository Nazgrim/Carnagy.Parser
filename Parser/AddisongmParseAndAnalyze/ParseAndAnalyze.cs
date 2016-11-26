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
        private IBaseRepository Repository { get; set; }
        private IDownloadImage DownloadImage { get; set; }
        private IDownloadManager DownloadManager { get; set; }

        public ParseAndAnalyze(IBaseRepository repository, IDownloadImage downloadImage, IDownloadManager downloadManager)
        {
            Repository = repository;
            DownloadImage = downloadImage;
            DownloadManager = downloadManager;
        }

        public void Run()
        {
            var rooobject = GetRootobject();
            var dealer = Repository.GetDealerByName("addisongm");
            if (dealer == null)
                return;

            var vehicles = rooobject.vehicles.ToList();
            var stockNumbers = Repository.GetAllStockNumber(dealer.Id);
            FillDatabase(rooobject.vehicles.ToList(), dealer, stockNumbers);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var images = Repository.GetCarsByDealerId(dealer.Id)
                .Select(a => new ImageDownloadCommand
                {
                    Id = a.Id,
                    Url = vehicles.FirstOrDefault(b => b.stocknumber == a.StockNumber)?.picture
                })
                .Where(a => !string.IsNullOrWhiteSpace(a.Url));
            DownloadImage.Download("Cars", images);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadKey();
        }

        private Rootobject GetRootobject()
        {
            var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            var path = System.IO.Path.Combine(sourceDirectory, "json\\inventory.json");
            var json = System.IO.File.ReadAllText(path);
            //var json = DownloadManager.DownloadString(InventoryUrl);
            var result = JsonConvert.DeserializeObject<Rootobject>(json);
            return result;
        }

        private void FillDatabase(List<Vehicle> vehicles, Dealer dealer, List<Car> stockNumbers)
        {
            var now = DateTime.Now;
            foreach (var vehicle in vehicles)
            {
                var car = stockNumbers.SingleOrDefault(a => a.StockNumber == vehicle.stocknumber);
                if (car == null)
                {
                    car = AddCar(vehicle, dealer);
                }
                else
                {
                    stockNumbers.Remove(car);
                }
                var advertCar = GetOrCreateAdvertCar(car);

                Repository.AddAdvertCarPrice(advertCar.Id, vehicle.price, now);
            }

            Repository.DeleteCars(stockNumbers);

            Repository.SaveChanges();
        }

        private Car AddCar(Vehicle vehicle, Dealer dealer)
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

            var car = GetCar(stockCar.Id, dealer.Id, vehicle.stocknumber);
            car = CreateOrUpdateCar(car, dealer.Id, stockCar.Id, vehicle.stocknumber, vehicle.price, vehicle);
            return car;
        }

        private AdvertCar GetOrCreateAdvertCar(Car car)
        {
            var advertCar = Repository.GetDealerAdvertCar(car.Id);
            if (advertCar != null)
                return advertCar;

            advertCar = new AdvertCar
            {
                MainAdvertCar = new MainAdvertCar { Car = car },
                IsDealer = true
            };
            Repository.CreateAdvertCar(advertCar);

            return advertCar;
        }

        private Car GetCar(int stockCarId, int dealerId, string stockNumber)
        {
            return Repository.GetCar(stockCarId, dealerId, stockNumber);
        }

        private Car CreateOrUpdateCar(Car car, int dealerId, int stockCarId, string stocknumber, float price, Vehicle vehicle)
        {
            if (car == null)
            {
                //формирование url взято у диллера на сайте
                var carUrl = $"http://addisongm.com/view/{vehicle.condition}-{vehicle.year}-{vehicle.make}-{vehicle.model}-{vehicle.id}";
                carUrl = System.Text.RegularExpressions.Regex.Replace(carUrl, @"\s g", "-").ToLower();
                car = new Car
                {
                    DealerId = dealerId,
                    StockCarId = stockCarId,
                    StockNumber = stocknumber,
                    Price = price,
                    Url = carUrl
                };
                Repository.CreateCar(car);
            }
            else
            {
                car.Price = price;
                Repository.SaveChanges();
            }
            return car;
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
