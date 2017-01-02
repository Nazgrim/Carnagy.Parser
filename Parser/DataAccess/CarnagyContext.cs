using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DataAccess.Configurations;
using DataAccess.Models;
using Configuration = DataAccess.Migrations.Configuration;

namespace DataAccess
{
    public class CarnagyContextFactory : IDbContextFactory<CarnagyContext>
    {
        public CarnagyContext Create()
        {
            var connection = "Data Source=USER-PC;Initial Catalog=Carnagy5;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            return new CarnagyContext(connection);
        }
    }

    public class CarnagyContext : DbContext
    {
        public DbSet<MainConfiguration> MainConfigurations { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldValue> FieldValues { get; set; }
        public DbSet<ParsedCar> ParsedCars { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Price> Prices { get; set; }

        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<Make> Makes { get; set; }
        public DbSet<Year> Years { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<BodyType> BodyTypes { get; set; }
        public DbSet<Drivetrain> Drivetrains { get; set; }
        public DbSet<StyleTrim> StyleTrims { get; set; }
        public DbSet<StockCar> StockCars { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<StockCarPrice> StockCarPrices { get; set; }
        public DbSet<AdvertCar> AdvertCars { get; set; }
        public DbSet<AdvertCarPrice> AdvertCarPrices { get; set; }
        public DbSet<MainAdvertCar> MainAdvertCars { get; set; }

        public CarnagyContext(string connnectionString = "DefaultConnection") : base(connnectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<CarnagyContext, Configuration>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new MainConfigurationConfiguration());
            modelBuilder.Configurations.Add(new ParsedCarConfiguration());
            modelBuilder.Configurations.Add(new FieldConfiguration());
            modelBuilder.Configurations.Add(new FieldValueConfiguration());
            modelBuilder.Configurations.Add(new ErrorLogConfiguration());
            modelBuilder.Configurations.Add(new PriceConfiguration());

            modelBuilder.Configurations.Add(new DealerConfiguration());
            modelBuilder.Configurations.Add(new MakeConfiguration());
            modelBuilder.Configurations.Add(new ModelConfiguration());
            modelBuilder.Configurations.Add(new YearConfiguration());
            modelBuilder.Configurations.Add(new BodyTypeConfiguration());
            modelBuilder.Configurations.Add(new DrivetrainConfiguration());
            modelBuilder.Configurations.Add(new StyleTrimConfiguration());
            modelBuilder.Configurations.Add(new StockCarConfiguration());
            modelBuilder.Configurations.Add(new CarConfiguration());
            modelBuilder.Configurations.Add(new StockCarPriceConfiguration());

            modelBuilder.Configurations.Add(new AdvertCarConfuguration());
            modelBuilder.Configurations.Add(new AdvertCarPriceConfiguration());
            modelBuilder.Configurations.Add(new MainAdvertCarConfiguration());
        }
    }
}
