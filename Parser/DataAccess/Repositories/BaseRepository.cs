using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
