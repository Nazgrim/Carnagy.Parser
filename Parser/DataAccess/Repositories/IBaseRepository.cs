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

        Dealer GetDealerById(int id);
        List<Car> GetCarsByDealerId(int dealerId);
        Make GetMakersByValue(string make);       
        Model GetModelsByValue(string model);
        Year GetYearByValue(string year);        
        BodyType GetBodyTypeByValue(string bodyType);
        Drivetrain GetDrivetrainsByValue(string drivetrain);       
        StyleTrim GetStyleTrimsByValue(string styleTrim);       
        StockCar GetStockCar(Func<StockCar, bool> filter);       
        void AddCar(Car car);

        //void AddStockCar(StockCar stockCar);
        //void AddYear(Year year);
        //void AddMaker(Make make);
        //void AddModel(Model model);
        //void AddBodyType(BodyType bodyType);
        //void AddDrivetrain(Drivetrain drivetrain);
        //void AddStyleTrim(StyleTrim styleTrim);
        Car GetCarById(int carId);
        List<Dealer> GetDealers(Func<Dealer, bool> filter);
        StockCar GetStockCarWithPrices(int stockCarId);
    }
}
