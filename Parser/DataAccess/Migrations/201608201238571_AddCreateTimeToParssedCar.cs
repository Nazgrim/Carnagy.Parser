namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreateTimeToParssedCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParssedCars", "CreatedTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParssedCars", "CreatedTime");
        }
    }
}
