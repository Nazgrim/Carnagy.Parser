namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageSrcFiledToStockCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockCars", "ImageScr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockCars", "ImageScr");
        }
    }
}
