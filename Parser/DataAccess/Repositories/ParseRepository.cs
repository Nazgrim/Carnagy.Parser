using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class ParseRepository : IParseRepository
    {
        private CarnagyContext Context { get; set; }

        public ParseRepository(CarnagyContext context)
        {
            Context = context;
            context.Configuration.AutoDetectChangesEnabled = false;
        }

        public void SaveParsedCar(List<ParsedCar> parsedCars)
        {
            Context.ParsedCars.AddRange(parsedCars);
        }

        public void AddFieldValues(List<FieldValue> fieldValues)
        {
            Context.FieldValues.AddRange(fieldValues);
        }

        public MainConfiguration GetMainConfigurationByName(string name)
        {
            return Context.MainConfigurations.SingleOrDefault(a => a.Name == name);
        }

        public List<ParsedCar> GetParsedCars(Func<ParsedCar, bool> filter)
        {
            return Context.ParsedCars.Include(a=>a.Prices).Where(filter).ToList();
        }

        public void AddPrices(IEnumerable<Price> prices)
        {
            Context.Set<Price>().AddRange(prices);
        }

        public void RemoveParsedCar(IEnumerable<ParsedCar> other)
        {
            Context.ParsedCars.RemoveRange(other);
        }

        public void AddErrorLog(List<ErrorLog> errorLog)
        {
            Context.ErrorLogs.AddRange(errorLog);
        }

        public void SaveChanges()
        {
            Context.Configuration.AutoDetectChangesEnabled = true;
            Context.SaveChanges();
            Context.Configuration.AutoDetectChangesEnabled = false;
        }

        public void ClearParsed()
        {
            Context.ParsedCars.RemoveRange(Context.ParsedCars.ToList());
            Context.ErrorLogs.RemoveRange(Context.ErrorLogs.ToList());
            Context.SaveChanges();
        }
    }
}
