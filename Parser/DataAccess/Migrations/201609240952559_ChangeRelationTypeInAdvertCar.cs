namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRelationTypeInAdvertCar : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdvertCarPrices", "AdvertCarId", "dbo.AdvertCars");
            DropIndex("dbo.AdvertCars", new[] { "ParsedCarId" });
            DropPrimaryKey("dbo.AdvertCars");
            AddColumn("dbo.AdvertCars", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.AdvertCars", "ParsedCarId", c => c.Int());
            AddPrimaryKey("dbo.AdvertCars", "Id");
            CreateIndex("dbo.AdvertCars", "ParsedCarId");
            AddForeignKey("dbo.AdvertCarPrices", "AdvertCarId", "dbo.AdvertCars", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvertCarPrices", "AdvertCarId", "dbo.AdvertCars");
            DropIndex("dbo.AdvertCars", new[] { "ParsedCarId" });
            DropPrimaryKey("dbo.AdvertCars");
            AlterColumn("dbo.AdvertCars", "ParsedCarId", c => c.Int(nullable: false));
            DropColumn("dbo.AdvertCars", "Id");
            AddPrimaryKey("dbo.AdvertCars", "ParsedCarId");
            CreateIndex("dbo.AdvertCars", "ParsedCarId");
            AddForeignKey("dbo.AdvertCarPrices", "AdvertCarId", "dbo.AdvertCars", "ParsedCarId", cascadeDelete: true);
        }
    }
}
