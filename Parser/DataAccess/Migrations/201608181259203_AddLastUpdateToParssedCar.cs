namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastUpdateToParssedCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParssedCars", "LastUpdate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParssedCars", "LastUpdate");
        }
    }
}
