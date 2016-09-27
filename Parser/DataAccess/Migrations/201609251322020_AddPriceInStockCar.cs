namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPriceInStockCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockCars", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockCars", "Price");
        }
    }
}
