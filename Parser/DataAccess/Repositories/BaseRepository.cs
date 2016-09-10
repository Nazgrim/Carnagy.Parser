using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private CarnagyContext Context { get; set; }
        public BaseRepository(CarnagyContext context)
        {
            Context = context;
        }

        public void SaveParssedCar(List<ParssedCar> parssedCars)
        {
            Context.ParssedCars.AddRange(parssedCars);
            Context.SaveChanges();
        }

        public void ClearParssed()
        {
            Context.ParssedCars.RemoveRange(Context.ParssedCars.ToList());
            Context.ErrorLogs.RemoveRange(Context.ErrorLogs.ToList());
            Context.SaveChanges();
        }

        public void AddFiledsValue(List<FieldValue> fieldValues)
        {
            Context.FieldValues.AddRange(fieldValues);
            Context.SaveChanges();
        }

        public MainConfiguration GetMainConfigurationByName(string name)
        {
            return Context.MainConfigurations.SingleOrDefault(a => a.Name == name);
        }

        public List<ParssedCar> GetParssedCars(Func<ParssedCar, bool> filter)
        {
            return Context.ParssedCars.Where(filter).ToList();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public void AddErrorLog(List<ErrorLog> errorLog)
        {
            Context.ErrorLogs.AddRange(errorLog);
            Context.SaveChanges();
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
                .Where(a => a.DealerId == dealerId)
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
            return Context.StyleTrims.FirstOrDefault(a => a.Value == styleTrim);
        }

        public StockCar GetStockCar(Func<StockCar, bool> filter)
        {
            return Context.StockCars.FirstOrDefault(filter);
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
            return Context.StockCars
                .Include(a => a.Make)
                .Include(a => a.Model)
                .Include(a => a.Year)
                .Include(a => a.StockCarPrices)
                .FirstOrDefault(a => a.Id == stockCarId);
        }
    }
}
