namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStocCarPriceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockCarPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Value = c.Int(nullable: false),
                        StockCarId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StockCars", t => t.StockCarId, cascadeDelete: true)
                .Index(t => t.StockCarId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockCarPrices", "StockCarId", "dbo.StockCars");
            DropIndex("dbo.StockCarPrices", new[] { "StockCarId" });
            DropTable("dbo.StockCarPrices");
        }
    }
}
