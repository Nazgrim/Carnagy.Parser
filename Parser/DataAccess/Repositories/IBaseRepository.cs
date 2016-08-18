using System.Collections.Generic;
using DataAccess.Models;
using System;

namespace DataAccess.Repositories
{
    public interface IBaseRepository
    {
        List<MainConfiguration> GetMainConfigurations();
        void SaveParssedCar(List<ParssedCar> parssedCars);
        void AddFiledsValue(List<FieldValue> fieldValues);
        MainConfiguration GetMainConfigurationByName(string name);
        List<ParssedCar> GetParssedCars(Func<ParssedCar, bool> filter);
        void ClearParssed();
        void SaveChanges();
    }
}
