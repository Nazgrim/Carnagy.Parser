using System.Collections.Generic;
using DataAccess.Models;
using System;

namespace DataAccess.Repositories
{
    public interface IParseRepository
    {
        void SaveParsedCar(List<ParsedCar> parsedCars);
        void AddFieldValues(List<FieldValue> fieldValues);
        MainConfiguration GetMainConfigurationByName(string name);
        List<ParsedCar> GetParsedCars(Func<ParsedCar, bool> filter);
        void ClearParsed();
        void SaveChanges();
        void AddErrorLog(List<ErrorLog> errorLog);    
        void AddPrices(IEnumerable<Price> prices);
        void RemoveParsedCar(IEnumerable<ParsedCar> other);
    }
}
