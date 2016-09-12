using System;
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
                    Name = "AutoTrader",
                    SiteUrl = "http://www.autotrader.ca/",
                    HoursPeriond = 4,
                    CreateTime = DateTime.Now,
                    Fields = new List<Field>
                    {
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.PagerCurrentPage,Xpath =  "//*[@id='ctl00_ctl00_MainContent_MainContent_pager1_pnlPagerSection']/div/div/div[2]/span[contains(@class, 'PagerCurrentPage')]/following-sibling::a",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.List,Xpath = "//*[@id='adList']/div[contains(@class, 'at_featuredResult')]",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.ImgPath,Xpath = "div[1]/div[1]/div[2]/span[1]/a[1]/img[1]",Attribute = "src"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.DillerName,Xpath = "div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]",Attribute = "alt"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.DillerLogo,Xpath = "div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]",Attribute = "src"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Price,Xpath = "div[1]/div[1]/div[2]/div[2]/div[1]/text()[2]",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Distance,Xpath = "div[1]/div[1]/div[2]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Url,Xpath = "div[1]/div[1]/div[2]/span[1]/a[1]",Attribute = "href",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Name,Xpath = "div[1]/div[2]/div[1]/h2[1]/a[1]/span[1]"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.DillerPlace,Xpath = "div[1]/div[2]/div[2]/div[1]/div[4]"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Description,Xpath = "div[1]/div[2]/div[1]/p[1]"},

                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Make           ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[1]/div[2]/span"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Model          ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[2]/div[2]/span"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Kilometres     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[3]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.BodyType       ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[4]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.StyleTrim      ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[5]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Engine         ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[6]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Cylinders      ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[7]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.StockNumber    ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[8]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Drivetrain     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[9]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.RWD            ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[1]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Transmission   ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[1]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.ExteriorColour ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.InteriorColour ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[3]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Passengers     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[4]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Doors          ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[5]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.FuelType       ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[6]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.HwyFuelEconomy ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[8]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.CityFuelEconomy,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[7]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Description    ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecifications_ownerReviewGlance_ratingAverage_suffix']"},
                    }
                });
            var stockCar = new StockCar
            {
                Year = new Year { Value = "2017" },
                Make = new Make { Value = "Ford" },
                Model = new Model { Value = "Fusion" },
                BodyType = new BodyType { Value = "SUV" },
                Drivetrain = new Drivetrain { Value = "RWD" },
                StyleTrim = new StyleTrim { Value = "Premium" },
                ImageScr = "../app/img/f3q.jpg",
                StockCarPrices = new List<StockCarPrice>
                {
                    new StockCarPrice { DateTime = DateTime.Now, Value =39 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(1), Value =42 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(2), Value =57 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(3), Value =85 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(4), Value =119 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(5), Value =152 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(6), Value =170 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(7), Value =166 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(8), Value =142 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(9), Value =103 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(10), Value =66 },
                    new StockCarPrice { DateTime = DateTime.Now.AddDays(11), Value =48 }
                }
            };
            context.StockCars.Add(stockCar);
            context.Dealers.AddOrUpdate(new Dealer
            {
                Cars = new List<Car>
                {
                    new Car
                    {
                        Url = "dealer.com",
                        StockCar = stockCar,
                        Price = 7325
                    }
                }
            });
            context.Dealers.AddRange(new List<Dealer>
            {
                new Dealer
                {
                    Name = "Dealer1",
                    Location = "London",
                    WebSiteName = "dealer.com",
                    WebSireUrl = "dealer.com",
                    Cars = new List<Car>
                    {
                        new Car { Url = "dealer.com", StockCar = stockCar, Price = 7325 },
                        new Car { Url = "dealer.com", StockCar = stockCar, Price = 7325 },
                        new Car { Url = "dealer.com", StockCar = stockCar, Price = 7325 },
                    }
                },
                new Dealer
                {
                    Name = "Dealer2",
                    Location = "London",
                    WebSiteName = "dealer.com",
                    WebSireUrl = "dealer.com",
                    Cars = new List<Car>
                    {
                        new Car { Url = "dealer.com", StockCar = stockCar, Price = 7325 },
                        new Car { Url = "dealer.com", StockCar = stockCar, Price = 7325 },
                        new Car { Url = "dealer.com", StockCar = stockCar, Price = 7325 },
                    }
                }
            });
        }
    }
}
