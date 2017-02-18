using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class AnalyseRepository : IAnalyseRepository
    {
        private CarnagyContext Context { get; set; }

        public AnalyseRepository(CarnagyContext context)
        {
            Context = context;
        }

        public List<Car> GetAllStockNumber(int dealerId)
        {
            var result = Context.Set<Car>()
                .Include(a => a.MainAdvertCar)
                .Where(a => a.DealerId == dealerId && !a.MainAdvertCar.IsDeleted)
                .ToList();
            return result;
        }

        public void DeleteCars(List<Car> cars, DateTime deletedTime)
        {
            foreach (var car in cars)
            {
                car.MainAdvertCar.IsDeleted = true;
                car.DeletedTime = deletedTime;
            }
        }

        public List<ParsedCar> GetParsedCarsByPage(int skip, int take, int configurationId)
        {
            return Context.Set<ParsedCar>()
                .Where(a =>
                //!a.IsDeleted
                //&& 
                (a.Status == ParsedCarStatus.Page || a.Status == ParsedCarStatus.AnalyzeComplete)
                //|| a.Status == ParsedCarStatus.CantGetStockCar || a.Status == ParsedCarStatus.CannotParsePrice
                && a.MainConfigurationId == configurationId)
                .OrderBy(a => a.Id)
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        public void DetachParsedCars(List<ParsedCar> parrsedCar)
        {
            foreach (var parsedCar in parrsedCar)
            {
                Context.Entry(parsedCar).State = EntityState.Detached;
            }
        }

        public AdvertCar GetAdvertCar(int parsedCarId)
        {
            return Context.Set<AdvertCar>().FirstOrDefault(a => a.ParsedCarId == parsedCarId);
        }

        public Dealer GetDealerById(int id)
        {
            return Context.Set<Dealer>()
                .SingleOrDefault(a => a.Id == id);
        }

        public List<Car> GetCarsByDealerId(int dealerId)
        {
            return Context.Set<Car>()
                .Include(a => a.StockCar.BodyType)
                .Include(a => a.StockCar.Drivetrain)
                .Include(a => a.StockCar.Make)
                .Include(a => a.StockCar.Model)
                .Include(a => a.StockCar.StyleTrim)
                .Include(a => a.StockCar.Year)
                .Where(a => a.DealerId == dealerId && !a.MainAdvertCar.IsDeleted)                
                .ToList();
        }

        public Make GetMakersByValue(string make)
        {
            return Context.Makes.FirstOrDefault(a => a.Value == make);
        }

        public Model GetModelsByValue(string model)
        {
            return Context.Models.FirstOrDefault(a => a.Value == model);
        }

        public Year GetYearByValue(string year)
        {
            return Context.Years.FirstOrDefault(a => a.Value == year);
        }

        public BodyType GetBodyTypeByValue(string bodyType)
        {
            return Context.BodyTypes.FirstOrDefault(a => a.Value == bodyType);
        }

        public Drivetrain GetDrivetrainsByValue(string drivetrain)
        {
            return Context.Drivetrains.FirstOrDefault(a => a.Value == drivetrain);
        }

        public StyleTrim GetStyleTrimsByValue(string styleTrim)
        {
            return Context.Set<StyleTrim>().FirstOrDefault(a => a.Value == styleTrim);
        }

        public List<StockCar> GetStockCars(Func<StockCar, bool> filter)
        {
            return Context.Set<StockCar>().Where(filter).ToList();
        }

        public StockCar GetStockCar(Func<StockCar, bool> filter)
        {
            return Context.Set<StockCar>().SingleOrDefault(filter);
        }

        public List<StockCar> GetStockCars()
        {
            return Context.Set<StockCar>()
                .Include(a => a.Cars.Select(b => b.MainAdvertCar.AdvertCars.Select(c => c.AdvertCarPrices)))
                .ToList();
        }

        public void AddCar(Car car)
        {
            Context.Set<Car>().Add(car);
            Context.SaveChanges();
        }

        public Car GetCarById(int carId)
        {
            return Context.Set<Car>()
                .Include(a => a.StockCar.BodyType)
                .Include(a => a.StockCar.Drivetrain)
                .Include(a => a.StockCar.Make)
                .Include(a => a.StockCar.Model)
                .Include(a => a.StockCar.StyleTrim)
                .Include(a => a.StockCar.Year)
                .FirstOrDefault(a => a.Id == carId);
        }

        public List<Dealer> GetDealers(Func<Dealer, bool> filter)
        {
            return Context.Set<Dealer>()
                .Include(d => d.Cars.Select(a => a.StockCar))
                .Where(filter)
                .ToList();
        }

        public StockCar GetStockCarWithPrices(int stockCarId)
        {
            return Context.Set<StockCar>()
                .Include(a => a.Make)
                .Include(a => a.Model)
                .Include(a => a.Year)
                .Include(a => a.StockCarPrices)
                .FirstOrDefault(a => a.Id == stockCarId);
        }

        public List<MainConfiguration> GetConfigurations()
        {
            return Context.Set<MainConfiguration>().ToList();
        }

        public void CreateAdvertCar(AdvertCar advertCar)
        {
            Context.Set<AdvertCar>().Add(advertCar);
            Context.SaveChanges();
        }

        public void AddAdvertCarPrice(int advertCarId, double value, DateTime now)
        {
            Context.Set<AdvertCarPrice>()
                .Add(new AdvertCarPrice
                {
                    AdvertCarId = advertCarId,
                    Value = value,
                    DateTime = now
                });
            Context.SaveChanges();
        }

        public T GetDictionaryEntity<T>(string value)
            where T : class, IDictionaryEntity
        {
            return Context.Set<T>().FirstOrDefault(a => a.Value == value);
        }

        public List<T> GetDictionaryEntity<T>() where T : class, IDictionaryEntity
        {
            return Context.Set<T>().ToList();
        }

        public Dealer GetDealerByWebSireUrl(string url)
        {
            return Context.Set<Dealer>().FirstOrDefault(a => a.WebSireUrl == url);
        }

        public void CreateDealer(Dealer dealer)
        {
            Context.Set<Dealer>().Add(dealer);
            Context.SaveChanges();
        }

        public void CreateDictionary<T>(T dictionary) where T : class, IDictionaryEntity
        {
            Context.Set<T>().Add(dictionary);
            Context.SaveChanges();
        }

        public void CreateStockCar(StockCar stockCar)
        {
            Context.Set<StockCar>().Add(stockCar);
            Context.SaveChanges();
        }

        public Car GetCar(int stockCarId, int dealerId, string stockNumber)
        {
            return Context.Set<Car>()
                .Include(a => a.MainAdvertCar)
                .FirstOrDefault(a => a.StockCarId == stockCarId && a.DealerId == dealerId && a.StockNumber == stockNumber);
        }

        public Car GetCarByStockNumber(string stockNumber, int dealerId)
        {
            return Context.Set<Car>()
                .Include(a => a.MainAdvertCar)
                .SingleOrDefault(a => a.StockNumber == stockNumber && a.DealerId == dealerId);
        }

        public void DeleteStockCar(StockCar stockCar)
        {
            Context.Set<StockCar>().Remove(stockCar);
        }

        public void CreateCar(Car car)
        {
            Context.Set<Car>().Add(car);
            Context.SaveChanges();
        }

        public List<AdvertCar> GetAdvertCars(int carId)
        {
            return Context.Set<AdvertCar>().Where(a => a.MainAdvertCarId == carId).ToList();
        }

        public List<Car> GetStockCarPrices(int stockCarId)
        {
            return Context.Set<Car>().Where(a => a.StockCarId == stockCarId).ToList();
        }

        public List<Car> GetCarsByStockCarId(int stockCarId)
        {
            return Context.Set<Car>()
                .Include(a => a.Dealer)
                .Where(a => a.StockCarId == stockCarId)
                .ToList();
        }

        public List<Car> GetCarsByFilter(Func<Car, bool> filter)
        {
            return Context.Set<Car>()
                .Include(a => a.StockCar)
                .Include(a => a.Dealer)
                .Where(filter)
                .ToList();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
