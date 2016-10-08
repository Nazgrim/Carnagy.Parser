namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMsrpPriceFieldInStockCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockCars", "MsrpPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockCars", "MsrpPrice");
        }
    }
}
