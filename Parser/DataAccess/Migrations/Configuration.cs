using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;

namespace DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CarnagyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CarnagyContext context)
        {
            context.MainConfigurations.AddOrUpdate(m => m.Id,
                new MainConfiguration
                {
                    Id = 1,
                    Name = "AutoTrader",
                    SiteUrl = "http://www.autotrader.ca/",
                    HoursPeriond = 4,
                    CreateTime = DateTime.Now,
                    Fields = new List<Field>
                    {
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.PagerCurrentPage,Xpath =  "//*[@id='ctl00_ctl00_MainContent_MainContent_pager1_pnlPagerSection']/div/div/div[2]/span[contains(@class, 'PagerCurrentPage')]/following-sibling::a",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.List,Xpath = "//*[@id='adList']/div[contains(@class, 'at_result')]",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.ImgPath,Xpath = "div[1]/div[1]/div[2]/span[1]/a[1]/img[1]",Attribute = "src"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Price,Xpath = "div[1]/div[1]/div[2]/div[2]/div[1]/text()[2],div[1]/div[1]/div[2]/div[2]/div[1]/span[1]",IsDefault = true, RegExPattern = @"\$(\d{1,3},?)+.?\d{2}?"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Distance,Xpath = "div[1]/div[1]/div[2]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Url,Xpath = "div[1]/div[1]/div[2]/span[1]/a[1]",Attribute = "href",IsDefault = true},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Name,Xpath = "div[1]/div[2]/div[1]/h2[1]/a[1]/span[1]"},
                        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.Description,Xpath = "div[1]/div[2]/div[1]/p[1]"},
                        //xpath для получении информации о дилере со страницы списка, данные не используются потому что не точные  
                        //new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.DealerPlace,Xpath = "div[1]/div[2]/div[2]/div[1]/div[4]"},
                        //new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.DealerName,Xpath = "div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]",Attribute = "alt"},
                        //new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.DealerLogo,Xpath = "div[1]/div[2]/div[2]/div[1]/div[1]/a[1]/img[1]",Attribute = "src"},
                        
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.DealerPlace    ,Xpath = "//*[@id='transparencyDealerTrustContainer']/div[3]/ul"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.DealerName     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptNewVehicleDetailsPage_ctl00_detailsView_ContactSellerV3AtRightSideBar_dealerTrust_companyLogo']",Attribute = "alt"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.DealerWebSite  ,Xpath = "//*[@id='dealerWebsiteLink']",Attribute = "href"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.DealerLogo     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptNewVehicleDetailsPage_ctl00_detailsView_ContactSellerV3AtRightSideBar_dealerTrust_companyLogo']",Attribute = "src"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Make           ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[1]/div[2]/span"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Model          ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[2]/div[2]/span"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Kilometres     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[3]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[1]/div[2],//*[@id='topSpecs']/div[2]/div[1]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.BodyType       ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[4]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[4]/div[2],//*[@id='topSpecs']/div[1]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.StyleTrim      ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[5]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[3]/div[2],//*[@id='topSpecs']/div[2]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Engine         ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[6]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[5]/div[2],//*[@id='specs']/div[1]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Cylinders      ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[7]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[6]/div[2],//*[@id='specs']/div[1]/div[3]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.StockNumber    ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[8]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[1]/div[2],//*[@id='specs']/div[1]/div[5]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Drivetrain     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[1]/div[9]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[8]/div[2],//*[@id='specs']/div[1]/div[4]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Transmission   ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[1]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[1]/div[7]/div[2],//*[@id='topSpecs']/div[1]/div[1]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.ExteriorColour ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[2]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.InteriorColour ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[3]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[3]/div[2],//*[@id='specs']/div[2]/div[1]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Passengers     ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[4]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[4]/div[2],//*[@id='specs']/div[2]/div[2]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Doors          ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[5]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[5]/div[2],//*[@id='specs']/div[2]/div[3]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.FuelType       ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[6]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[6]/div[2],//*[@id='specs']/div[2]/div[4]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.HwyFuelEconomy ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[8]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[7]/div[2],//*[@id='specs']/div[2]/div[6]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.CityFuelEconomy,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecificationsPanel']/div/div/div[2]/div[2]/div[7]/div[2],//*[@id='transparencySpecsContainer']/div[1]/div[2]/div[7]/div[2],//*[@id='specs']/div[2]/div[5]/div[2]"},
                        new Field {ConfigurationType =FieldConfigurationType.Page, Name = FiledNameConstant.Description    ,Xpath = "//*[@id='ctl00_ctl00_MainContent_MainContent_rptAdDetail_ctl00_adDetailControl_vehicleSpecifications_ownerReviewGlance_ratingAverage_suffix']"},
                    }
                });
            context.Dealers.AddOrUpdate(m => m.Id, new Dealer
            {
                Id = 1,
                Name = "addisongm",
                IsCreated = true,
                Logo = "logo.jpg",
                WebSireUrl = "http://www.addisongm.com",
                WebSiteName = "addisongm.com",
                Location = "1220 Eglinton Ave East Mississauga, ON L4W 2M7",

            });
            AddYears(context);
            AddMakers(context);
            AddModels(context);
            AddBodyTypes(context);
            AddDrivetrain(context);
            AddStyleTrim(context);


            //context.MainConfigurations.AddOrUpdate(m => m.Id, new MainConfiguration
            //{
            //    Id = 2,
            //    Name = "addisongm",
            //    SiteUrl = "http://addisongm.com/",
            //    CreateTime = DateTime.Now,
            //    HoursPeriond = 4,
            //    Dealer = new Dealer
            //    {

            //    },
            //    Fields = new List<Field>
            //    {
            //        new Field {ConfigurationType =FieldConfigurationType.List, Name = FiledNameConstant.PagerCurrentPage,Xpath =  "//*[@id='ctl00_ctl00_MainContent_MainContent_pager1_pnlPagerSection']/div/div/div[2]/span[contains(@class, 'PagerCurrentPage')]/following-sibling::a", IsDefault = true},
            //    }
            //});

            //var stockCar = new StockCar
            //{
            //    Year = new Year { Value = "2017" },
            //    Make = new Make { Value = "Ford" },
            //    Model = new Model { Value = "Fusion" },
            //    BodyType = new BodyType { Value = "SUV" },
            //    Drivetrain = new Drivetrain { Value = "RWD" },
            //    StyleTrim = new StyleTrim { Value = "Premium" },
            //    ImageScr = "../app/img/f3q.jpg",
            //    StockCarPrices = new List<StockCarPrice>
            //    {
            //        new StockCarPrice { DateTime = DateTime.Now, Value =39 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(1), Value =42 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(2), Value =57 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(3), Value =85 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(4), Value =119 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(5), Value =152 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(6), Value =170 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(7), Value =166 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(8), Value =142 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(9), Value =103 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(10), Value =66 },
            //        new StockCarPrice { DateTime = DateTime.Now.AddDays(11), Value =48 }
            //    }
            //};
            //context.StockCars.AddOrUpdate(m=>m.ImageScr, stockCar);
            //context.Dealers.AddOrUpdate();
            //var dealer0 = new Dealer
            //{
            //    Name = "Dealer0",
            //    Cars = new List<Car>
            //    {
            //        new Car
            //        {
            //            Url = "dealer.com",
            //            StockCar = stockCar,
            //            Price = 7325
            //        }
            //    }
            //};
            //var dealer1 = new Dealer
            //{
            //    Name = "Dealer1",
            //    Location = "London",
            //    WebSiteName = "dealer.com",
            //    WebSireUrl = "dealer.com",
            //    Cars = new List<Car>
            //    {
            //        new Car {Url = "dealer.com", StockCar = stockCar, Price = 7325},
            //        new Car {Url = "dealer.com", StockCar = stockCar, Price = 7325},
            //        new Car {Url = "dealer.com", StockCar = stockCar, Price = 7325},
            //    }
            //};
            //var dealer2 = new Dealer
            //{
            //    Name = "Dealer2",
            //    Location = "London",
            //    WebSiteName = "dealer.com",
            //    WebSireUrl = "dealer.com",
            //    Cars = new List<Car>
            //    {
            //        new Car {Url = "dealer.com", StockCar = stockCar, Price = 7325},
            //        new Car {Url = "dealer.com", StockCar = stockCar, Price = 7325},
            //        new Car {Url = "dealer.com", StockCar = stockCar, Price = 7325},
            //    }
            //};
            //context.Dealers.AddOrUpdate(a => a.Name, dealer0);
            //context.Dealers.AddOrUpdate(a => a.Name, dealer1);
            //context.Dealers.AddOrUpdate(a => a.Name, dealer2);
        }

        private void AddStyleTrim(CarnagyContext context)
        {
            var styleTrimNames = new List<string>
            {
                "Not know",
                "Premium",
                "Premium I",
                "Premium II",
                "Convenience",
                "Convenience 1",
                "Essence",
                "Leather",
                "Sport Touring",
                "Preferred",
                "Base"
            };
            context.StyleTrims.AddOrUpdate(b => b.Id, styleTrimNames.Select((b, index) => new StyleTrim { Id = index + 1, Value = b }).ToArray());
        }

        private void AddDrivetrain(CarnagyContext context)
        {
            var drivetrainNames = new List<string>
            {
                "Not know",
                "4x4",
                "AWD",
                "FWD",
                "RWD"
            };
            context.Drivetrains.AddOrUpdate(b => b.Id, drivetrainNames.Select((b, index) => new Drivetrain { Id = index + 1, Value = b }).ToArray());
        }

        private void AddBodyTypes(CarnagyContext context)
        {
            var bodyTypeNames = new List<string>
            {
                "Not know",
                "Convertible",
                "Coupe",
                "Hatchback",
                "Minivan",
                "Sedan",
                "SUV",
                "Truck",
                "Wagon",
                "2dr Car",
                "Cargo Van",
                "Crew Cab",
                "Crew Cab Pickup",
                "Crew Cab Pickup - Short Bed",
                "Crew Cab Pickup - Standard Bed",
                "Crew Pickup",
                "Extended Cab Pickup",
                "Extended Cab Pickup - Standard Bed",
                "Extended Cargo Van",
                "Full-size Cargo Van",
                "Mini-van, Passenger",
                "Other/Don't Know",
                "Pick-up",
                "Regular Cab Pickup",
                "Regular Cargo",
                "Sedan 4 Dr.",
                "Station Wagon",
                "Wagon 4 Dr."
            };
            context.BodyTypes.AddOrUpdate(b => b.Id, bodyTypeNames.Select((b, index) => new BodyType { Id = index + 1, Value = b }).ToArray());
        }

        private void AddModels(CarnagyContext context)
        {
            var modelNames = new List<string>
            {
                "Not know",
                //Buick
                "Allure", "Cascada", "Century", "Electra", "Enclave", "Encore", "Envision", "Gran Sport", "GRAND NATIONAL", "LaCrosse", "LeSabre",
                "Lucerne", "Park Avenue", "Rainier", "Reatta", "Regal", "Rendezvous", "Riviera", "Roadmaster", "Skylark", "Special", "Terraza",
                "Unspecified", "Verano", "Wildcat",

                //Chevrolet
                 "1300", "150", "1500 Pickup", "20 Pickup", "2500", "30 Pickup", "3100 Pickup", "3500 Pickup", "3600", "5500 Pickup", "AG Master Deluxe",
                "Apache", "ASTRO", "Avalanche", "Aveo", "AVEO 5", "Bel Air", "Biscayne", "Blazer", "C/K 2500", "C10", "C1500", "C1500 Pick-up", "Camaro",
                "Cameo", "Caprice", "Captiva", "Cavalier", "Chevelle", "Chevette", "Chevy II", "CHEYENNE", "City Express", "Cobalt", "Colorado", "Corsica",
                "Corvair", "Corvette", "Cruze", "Deluxe", "El Camino", "Epica", "Equinox", "HHR", "Impala", "Lumina", "Malibu", "Malibu Hybrid", "MALIBU MAXX",
                "Master", "Monte Carlo", "Nova", "Optra", "OPTRA 5", "Optra Sedan", "Optra Wagon", "Orlando", "S10", "S10 Blazer", "Silverado", "Silverado 1500",
                "Silverado 2500", "Silverado 3500", "SILVERADO 3500HD", "Sonic", "Spark", "Spark EV", "Sportvan", "Sprint", "SSR", "Starcraft Conversion Van", "Styleline",
                "Stylemaster", "Suburban", "SUPERIOR", "Tahoe", "Tahoe Hybrid", "Tracker", "TrailBlazer", "Traverse", "Trax", "Uplander", "Venture", "Volt Electric",
                
                //GMC
                "1000", "150 Pickup", "250 Pickup", "2500 Cab-Chassis", "2500 Pickup", "350 Pickup", "4500 Pickup", "910", "Acadia", "Acadia Denali",
                "C10 Pickup", "Canyon", "DENALI", "Envoy", "Jimmy", "New Sierra 1500", "New Sierra 2500", "S15 Jimmy", "S15 Pickup", "Safari", "Savana 1500 Passenger",
                "Savana 2500 Passenger", "Sierra 1500", "Sierra 1500 Denali", "Sierra 2500", "Sierra 2500 Denali HD", "Sierra 3500", "Sierra 3500 Denali HD", "Sierra 3500HD",
                "Sonoma", "T-Series", "Terrain", "Terrain Denali", "Yukon  Denali", "Yukon", "Yukon Hybrid", "Yukon XL", "Yukon XL Denali"
            };

            context.Models.AddOrUpdate(a => a.Id, modelNames.Select((b, index) => new Model { Id = index + 1, Value = b }).ToArray());
        }

        private void AddMakers(CarnagyContext context)
        {
            var makeNames = new List<string>
            {
                "Not know",
                "Chevrolet",
                "Buick",
                "GMC",
                "Hyundai",
                "Ford",
                "Dodge",
                "Toyota",
                "Nissan",
                "Jeep",
                "Mazda",
                "Subaru",
                "Cadillac",
                "Mercedes-Benz",
                "RAM",
                "Lexus",
                "BMW",
                "Jaguar",
                "Volkswagen",
                "Kia",
                "Honda",
                "Chrysler",
                "Infiniti",
                "Audi",
                "Pontiac",
            };

            context.Makes.AddOrUpdate(a => a.Id, makeNames.Select((b, index) => new Make() { Id = index + 1, Value = b }).ToArray());
        }

        private void AddYears(CarnagyContext context)
        {
            context.Years.AddOrUpdate(a => a.Id, Enumerable.Range(1900, 120).Select((b, index) => new Year { Id = index + 1, Value = b.ToString() }).ToArray());
        }
    }
}
