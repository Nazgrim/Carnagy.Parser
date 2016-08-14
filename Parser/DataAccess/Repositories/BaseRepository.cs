﻿using System.Collections.Generic;
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
            return _context.MainConfigurations.ToList();
        }

        public void SaveParssedCar(List<ParssedCar> parssedCars)
        {
            _context.ParssedCars.AddRange(parssedCars);
            _context.SaveChanges();
        }

        public void ClearParssed()
        {
            _context.ParssedCars.RemoveRange(_context.ParssedCars.ToList());
        }
    }
}
