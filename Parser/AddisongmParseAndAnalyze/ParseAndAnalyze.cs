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
        private IBaseRepository Repository { get; set; }
        private IDownloadImage DownloadImage { get; set; }
        public ParseAndAnalyze(IBaseRepository repository, IDownloadImage downloadImage)
        {
            Repository = repository;
            DownloadImage = downloadImage;
        }

        public void Run()
        {
            var rooobject = GetRootobject();
            var dealer = Repository.GetDealerByName("addisongm");
            if (dealer == null)
                return;

            var vehicles = rooobject.vehicles.ToList();
            //FillDatabase(rooobject.vehicles.ToList(), dealer);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var images = Repository.GetCarsByDealerId(dealer.Id)
                .Select(a => new ImageForSave()
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

        private static Rootobject GetRootobject()
        {
            var sourceDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
            var path = System.IO.Path.Combine(sourceDirectory, "json\\inventory.json");
            var json = System.IO.File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<Rootobject>(json);
            return result;
        }

        private void FillDatabase(List<Vehicle> vehicles, Dealer dealer)
        {
            var now = DateTime.Now;
            foreach (var vehicle in vehicles)
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

                var stockCar = GetOrCreateStocCar(make, model, year, bodyType, styleTrim, drivetrain);

                var car = GetCar(stockCar.Id, dealer.Id, vehicle.stocknumber);
                car = CreateOrUpdateCar(car, dealer.Id, stockCar.Id, vehicle.stocknumber, vehicle.price, vehicle.id);

                var advertCar = GetOrCreateAdvertCar(car);

                Repository.AddAdvertCarPrice(advertCar.Id, vehicle.price, now);
            }
        }

        private AdvertCar GetOrCreateAdvertCar(Car car)
        {
            var advertCar = Repository.GetDealerAdvertCar(car.Id);
            if (advertCar != null)
                return advertCar;

            advertCar = new AdvertCar
            {
                Car = car,
                IsDealer = true,
            };
            Repository.CreateAdvertCar(advertCar);

            return advertCar;
        }

        private Car GetCar(int stockCarId, int dealerId, string stockNumber)
        {
            return Repository.GetCar(stockCarId, dealerId, stockNumber);
        }

        private Car CreateOrUpdateCar(Car car, int dealerId, int stockCarId, string stocknumber, float price, int carId)
        {
            if (car == null)
            {
                car = new Car
                {
                    DealerId = dealerId,
                    StockCarId = stockCarId,
                    StockNumber = stocknumber,
                    Price = price,
                    Url = $"http://addisongm.com/view/{carId}/"
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

        private StockCar GetOrCreateStocCar(Make make, Model model, Year year, BodyType bodyType, StyleTrim styleTrim, Drivetrain drivetrain)
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
