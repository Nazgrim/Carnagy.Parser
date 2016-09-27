namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStocNumberFieldInCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "StockNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "StockNumber");
        }
    }
}
