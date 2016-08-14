using DataAccess.Models;

namespace DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DataAccess.CarnagyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataAccess.CarnagyContext context)
        {
            context.MainConfigurations.AddOrUpdate(m => m.Name,
                new MainConfiguration { Name = "autotrader", SiteUrl = "http://www.autotrader.ca/", HoursPeriond = 4 });
        }
    }
}
