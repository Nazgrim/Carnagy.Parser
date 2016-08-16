using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private CarnagyContext _context { get; set; }
        public BaseRepository(CarnagyContext context)
        {
            _context = context;
        }

        public List<MainConfiguration> GetMainConfigurations()
        {
            return _context.MainConfigurations
                .Include(a=>a.Fields)
                .ToList();
        }



        public void SaveParssedCar(List<ParssedCar> parssedCars)
        {
            _context.ParssedCars.AddRange(parssedCars);
            _context.SaveChanges();
        }

        public void ClearParssed()
        {
            _context.ParssedCars.RemoveRange(_context.ParssedCars.ToList());
            _context.SaveChanges();
        }

        public void AddFiledsValue(List<FieldValue> fieldValues)
        {
            _context.FieldValues.AddRange(fieldValues);
            _context.SaveChanges();
        }
    }
}
