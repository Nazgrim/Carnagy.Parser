namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableAdvertCar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdvertCars",
                c => new
                    {
                        ParsedCarId = c.Int(nullable: false),
                        CarId = c.Int(nullable: false),
                        IsDealer = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ParsedCarId)
                .ForeignKey("dbo.Cars", t => t.CarId, cascadeDelete: true)
                .ForeignKey("dbo.ParsedCars", t => t.ParsedCarId)
                .Index(t => t.ParsedCarId)
                .Index(t => t.CarId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvertCars", "ParsedCarId", "dbo.ParsedCars");
            DropForeignKey("dbo.AdvertCars", "CarId", "dbo.Cars");
            DropIndex("dbo.AdvertCars", new[] { "CarId" });
            DropIndex("dbo.AdvertCars", new[] { "ParsedCarId" });
            DropTable("dbo.AdvertCars");
        }
    }
}
