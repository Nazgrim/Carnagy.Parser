using System.Collections.Generic;
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
                new MainConfiguration
                {
                    Name = "autotrader",
                    SiteUrl = "http://www.autotrader.ca/",
                    HoursPeriond = 4,
                    Fields = new List<Field>
                    {
                        new Field {Name = FiledNameConstant.ImgPath,Xpath = "div[1]/div[1]/div[2]/span[1]/a[1]/img[1]",Attribute = "src"},
                        new Field {Name = FiledNameConstant.DillerName,Xpath = "div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]",Attribute = "alt"},
                        new Field {Name = FiledNameConstant.DillerLogo,Xpath = "div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]",Attribute = "src"},
                        new Field {Name = FiledNameConstant.Price,Xpath = "div[1]/div[1]/div[2]/div[2]/div[1]/text()[2]"},
                        new Field {Name = FiledNameConstant.Distance,Xpath = "div[1]/div[1]/div[2]/div[2]/div[2]"},
                        new Field {Name = FiledNameConstant.Url,Xpath = "div[1]/div[1]/div[2]/span[1]/a[1]"},
                        new Field {Name = FiledNameConstant.Name,Xpath = "div[1]/div[2]/div[1]/h2[1]/a[1]/span[1]"},
                        new Field {Name = FiledNameConstant.DillerPlace,Xpath = "div[1]/div[2]/div[2]/div[1]/div[4]"},
                    }
                });
        }
    }
}
