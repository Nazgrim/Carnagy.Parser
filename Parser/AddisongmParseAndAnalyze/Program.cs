﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.Migrations;
using DataAccess.Models;
using DataAccess.Repositories;
using Newtonsoft.Json;

namespace AddisongmParseAndAnalyze
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new CarnagyContext())
            {
                var repository = new BaseRepository(context);
                var parseAndAnalyze = new ParseAndAnalyze(repository);
                parseAndAnalyze.Run();
            }
            //Console.ReadLine();
        }
    }

    public class ParseAndAnalyze
    {
        private IBaseRepository Repository { get; set; }
        public ParseAndAnalyze(IBaseRepository repository)
        {
            Repository = repository;
        }

        public void Run()
        {
            var rooobject = GetRootobject();
            var dealer = Repository.GetDealerByName("addisongm");
            if(dealer==null)
                return;

            FillDatabase(rooobject.vehicles.ToList(), dealer);
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
                var make = GetDictionaryOrCreateEntity<Make>(vehicle.make);
                var model = GetDictionaryOrCreateEntity<Model>(vehicle.model);
                var year = GetDictionaryOrCreateEntity<Year>(vehicle.year);
                var bodyType = GetDictionaryOrCreateEntity<BodyType>(vehicle.bodystyle);
                var styleTrim = GetDictionaryOrCreateEntity<StyleTrim>(vehicle.trim);
                var drivetrain = GetDictionaryOrCreateEntity<Drivetrain>(vehicle.drivetrain);

                var stockCar = GetOrCreateStocCar(make, model, year, bodyType, styleTrim, drivetrain);

                var car = GetCar(stockCar.Id, dealer.Id, vehicle.stocknumber);
                car=CreateOrUpdateCar(car, dealer.Id, stockCar.Id, vehicle.stocknumber, vehicle.price);

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

        private Car CreateOrUpdateCar(Car car, int dealerId, int stockCarId, string stocknumber, float price)
        {
            if (car == null)
            {
                car = new Car
                {
                    DealerId = dealerId,
                    StockCarId = stockCarId,
                    StockNumber = stocknumber,
                    Price = price,
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
