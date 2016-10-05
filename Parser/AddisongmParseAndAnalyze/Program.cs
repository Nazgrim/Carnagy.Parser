using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
                var baseDir = @"C:\Users\Иван\Source\Repos\Carnagy.Parser\Parser\WepApi\Image";
                var downloadImage = new DownloadImage(baseDir);
                var repository = new BaseRepository(context);
                var parseAndAnalyze = new ParseAndAnalyze(repository, downloadImage);
                parseAndAnalyze.Run();
            }
            //Console.ReadLine();
        }
    }

    public class ImageForSave
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class DownloadImage
    {
        private string BaseDir { get; set; }
        public DownloadImage(string baseDir)
        {
            BaseDir = baseDir;
        }

        public void Download(string dir, IEnumerable<ImageForSave> images)
        {
            var folder = Path.Combine(BaseDir, dir);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (var imageForSave in images)
            {
                var filePath = Path.Combine(folder, $"{imageForSave.Id}.jpg");
                if (File.Exists(filePath))
                    continue;
                DownloadRemoteImageFile(imageForSave.Url, filePath);
            }
        }

        private static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            var request = (HttpWebRequest)WebRequest.Create("http:" + uri);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Moved &&
                 response.StatusCode != HttpStatusCode.Redirect) ||
                !response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) return false;
            // if the remote file was found, download it
            using (var inputStream = response.GetResponseStream())
            using (var outputStream = File.OpenWrite(fileName))
            {
                var buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);
            }
            return true;
        }
    }

    public class ParseAndAnalyze
    {
        private IBaseRepository Repository { get; set; }
        private DownloadImage DownloadImage { get; set; }
        public ParseAndAnalyze(IBaseRepository repository, DownloadImage downloadImage)
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
