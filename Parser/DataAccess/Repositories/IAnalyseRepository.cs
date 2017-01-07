using System.Collections.Generic;
using DataAccess.Models;
using System;

namespace DataAccess.Repositories
{
    public interface IAnalyseRepository
    {
        void SaveChanges();      
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
        Dealer GetDealerByWebSireUrl(string url);
        void CreateDealer(Dealer dealer);
        void CreateDictionary<T>(T dictionary) where T : class, IDictionaryEntity;
        void CreateStockCar(StockCar stockCar);
        Car GetCar(int stockCarId, int dealerId, string stockNumber);
        void CreateCar(Car car);
        List<AdvertCar> GetAdvertCars(int carId);
        List<Car> GetCarsByStockCarId(int stockCarId);
        List<Car> GetCarsByFilter(Func<Car, bool> filter);
        List<Car> GetAllStockNumber(int dealerId);
        void DeleteCars(List<Car> cars, DateTime deletedTime);
        List<ParsedCar> GetParsedCarsByPage(int skip, int take, int configurationId);
        void DetachParsedCars(List<ParsedCar> parrsedCar);
        AdvertCar GetAdvertCar(int id);
        Dealer GetDealerById(int id);
        Car GetCarByStockNumber(string stockNumber, int dealerId);
        void DeleteStockCar(StockCar stockCar);
    }
}
