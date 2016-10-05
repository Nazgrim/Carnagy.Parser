using System.Collections.Generic;
using DataAccess.Models;
using System;

namespace DataAccess.Repositories
{
    public interface IBaseRepository
    {
        void SaveParsedCar(List<ParsedCar> parsedCars);
        void AddFieldValues(List<FieldValue> fieldValues);
        MainConfiguration GetMainConfigurationByName(string name);
        List<ParsedCar> GetParsedCars(Func<ParsedCar, bool> filter);
        void ClearParsed();
        void SaveChanges();
        void AddErrorLog(List<ErrorLog> errorLog);
      
        List<Car> GetCarsByDealerId(int dealerId);
        Make GetMakersByValue(string make);
        Model GetModelsByValue(string model);
        Year GetYearByValue(string year);
        BodyType GetBodyTypeByValue(string bodyType);
        Drivetrain GetDrivetrainsByValue(string drivetrain);
        StyleTrim GetStyleTrimsByValue(string styleTrim);
        StockCar GetStockCar(Func<StockCar, bool> filter);
        List<StockCar> GetStockCars();
        void AddCar(Car car);

        Car GetCarById(int carId);
        List<Dealer> GetDealers(Func<Dealer, bool> filter);
        StockCar GetStockCarWithPrices(int stockCarId);
        List<MainConfiguration> GetConfigurations();
        void CreateAdvertCar(AdvertCar advertCar);
        void AddAdvertCarPrice(int advertCarId, double value, DateTime now);
        T GetDictionaryEntity<T>(string value) where T : class, IDictionaryEntity;
        List<T> GetDictionaryEntity<T>() where T : class, IDictionaryEntity;
        //void CreateDealer(Dealer dealer);
        //Dealer GetDealerById(int id);
        //void AddStockCar(StockCar stockCar);
        //void AddYear(Year year);
        //void AddMaker(Make make);
        //void AddModel(Model model);
        //void AddBodyType(BodyType bodyType);
        //void AddDrivetrain(Drivetrain drivetrain);
        //void AddStyleTrim(StyleTrim styleTrim);
        Dealer GetDealerByName(string name);
        void CreateDealer(Dealer dealer);
        void CreateDictionary<T>(T dictionary) where T : class, IDictionaryEntity;
        void CreateStockCar(StockCar stockCar);
        Car GetCar(int stockCarId, int dealerId, string stockNumber);
        void CreateCar(Car car);
        AdvertCar GetDealerAdvertCar(int carId);
        List<double> GetStockCarPrices(int stockCarId);
    }
}
