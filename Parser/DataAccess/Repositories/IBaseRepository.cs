using System.Collections.Generic;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBaseRepository
    {
        List<MainConfiguration> GetMainConfigurations();
        void SaveParssedCar(List<ParssedCar> parssedCars);
        void AddFiledsValue(List<FieldValue> fieldValues);
        MainConfiguration GetMainConfigurationByName(string name);
        void ClearParssed();
    }
}
