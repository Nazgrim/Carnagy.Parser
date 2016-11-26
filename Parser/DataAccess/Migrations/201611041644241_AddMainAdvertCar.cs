namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMainAdvertCar : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdvertCars", "CarId", "dbo.Cars");
            DropIndex("dbo.AdvertCars", new[] { "CarId" });
            CreateTable(
                "dbo.MainAdvertCars",
                c => new
                    {
                        CarId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CarId)
                .ForeignKey("dbo.Cars", t => t.CarId)
                .Index(t => t.CarId);
            
            AddColumn("dbo.AdvertCars", "IsDealer", c => c.Boolean(nullable: false));
            AddColumn("dbo.AdvertCars", "MainAdvertCarId", c => c.Int(nullable: false));
            CreateIndex("dbo.AdvertCars", "MainAdvertCarId");
            AddForeignKey("dbo.AdvertCars", "MainAdvertCarId", "dbo.MainAdvertCars", "CarId", cascadeDelete: true);
            DropColumn("dbo.AdvertCars", "CarId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdvertCars", "CarId", c => c.Int(nullable: false));
            DropForeignKey("dbo.AdvertCars", "MainAdvertCarId", "dbo.MainAdvertCars");
            DropForeignKey("dbo.MainAdvertCars", "CarId", "dbo.Cars");
            DropIndex("dbo.MainAdvertCars", new[] { "CarId" });
            DropIndex("dbo.AdvertCars", new[] { "MainAdvertCarId" });
            DropColumn("dbo.AdvertCars", "MainAdvertCarId");
            DropColumn("dbo.AdvertCars", "IsDealer");
            DropTable("dbo.MainAdvertCars");
            CreateIndex("dbo.AdvertCars", "CarId");
            AddForeignKey("dbo.AdvertCars", "CarId", "dbo.Cars", "Id", cascadeDelete: true);
        }
    }
}
