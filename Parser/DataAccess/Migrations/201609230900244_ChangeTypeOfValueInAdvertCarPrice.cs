namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeOfValueInAdvertCarPrice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdvertCarPrices", "Value", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AdvertCarPrices", "Value", c => c.Int(nullable: false));
        }
    }
}
