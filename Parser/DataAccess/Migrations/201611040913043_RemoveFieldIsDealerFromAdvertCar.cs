namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFieldIsDealerFromAdvertCar : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AdvertCars", "IsDealer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AdvertCars", "IsDealer", c => c.Boolean(nullable: false));
        }
    }
}
