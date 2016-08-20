using DataAccess.Migrations;
using System.Data.Entity;
using DataAccess.Configurations;
using DataAccess.Models;

namespace DataAccess
{
    public class CarnagyContext : DbContext
    {
        public DbSet<MainConfiguration> MainConfigurations { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldValue> FieldValues { get; set; }
        public DbSet<ParssedCar> ParssedCars { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Price> Prices { get; set; }

        public CarnagyContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CarnagyContext, Configuration>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MainConfigurationConfiguration());
            modelBuilder.Configurations.Add(new ParssedCarConfiguration());
            modelBuilder.Configurations.Add(new FieldConfiguration());
            modelBuilder.Configurations.Add(new FieldValueConfiguration());
            modelBuilder.Configurations.Add(new ErrorLogConfiguration());
            modelBuilder.Configurations.Add(new PriceConfiguration());
        }
    }
}
