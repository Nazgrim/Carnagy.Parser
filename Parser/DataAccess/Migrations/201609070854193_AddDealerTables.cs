namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDealerTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BodyTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StockCars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MakeId = c.Int(nullable: false),
                        ModelId = c.Int(nullable: false),
                        YearId = c.Int(nullable: false),
                        BodyTypeId = c.Int(nullable: false),
                        StyleTrimId = c.Int(nullable: false),
                        DrivetrainId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BodyTypes", t => t.BodyTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Drivetrains", t => t.DrivetrainId, cascadeDelete: true)
                .ForeignKey("dbo.Makes", t => t.MakeId, cascadeDelete: true)
                .ForeignKey("dbo.Models", t => t.ModelId, cascadeDelete: true)
                .ForeignKey("dbo.StyleTrims", t => t.StyleTrimId, cascadeDelete: true)
                .ForeignKey("dbo.Years", t => t.YearId, cascadeDelete: true)
                .Index(t => t.MakeId)
                .Index(t => t.ModelId)
                .Index(t => t.YearId)
                .Index(t => t.BodyTypeId)
                .Index(t => t.StyleTrimId)
                .Index(t => t.DrivetrainId);
            
            CreateTable(
                "dbo.Cars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        StockCarId = c.Int(nullable: false),
                        DealerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dealers", t => t.DealerId, cascadeDelete: true)
                .ForeignKey("dbo.StockCars", t => t.StockCarId, cascadeDelete: true)
                .Index(t => t.StockCarId)
                .Index(t => t.DealerId);
            
            CreateTable(
                "dbo.Dealers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Drivetrains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Makes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Models",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StyleTrims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Years",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockCars", "YearId", "dbo.Years");
            DropForeignKey("dbo.StockCars", "StyleTrimId", "dbo.StyleTrims");
            DropForeignKey("dbo.StockCars", "ModelId", "dbo.Models");
            DropForeignKey("dbo.StockCars", "MakeId", "dbo.Makes");
            DropForeignKey("dbo.StockCars", "DrivetrainId", "dbo.Drivetrains");
            DropForeignKey("dbo.Cars", "StockCarId", "dbo.StockCars");
            DropForeignKey("dbo.Cars", "DealerId", "dbo.Dealers");
            DropForeignKey("dbo.StockCars", "BodyTypeId", "dbo.BodyTypes");
            DropIndex("dbo.Cars", new[] { "DealerId" });
            DropIndex("dbo.Cars", new[] { "StockCarId" });
            DropIndex("dbo.StockCars", new[] { "DrivetrainId" });
            DropIndex("dbo.StockCars", new[] { "StyleTrimId" });
            DropIndex("dbo.StockCars", new[] { "BodyTypeId" });
            DropIndex("dbo.StockCars", new[] { "YearId" });
            DropIndex("dbo.StockCars", new[] { "ModelId" });
            DropIndex("dbo.StockCars", new[] { "MakeId" });
            DropTable("dbo.Years");
            DropTable("dbo.StyleTrims");
            DropTable("dbo.Models");
            DropTable("dbo.Makes");
            DropTable("dbo.Drivetrains");
            DropTable("dbo.Dealers");
            DropTable("dbo.Cars");
            DropTable("dbo.StockCars");
            DropTable("dbo.BodyTypes");
        }
    }
}
