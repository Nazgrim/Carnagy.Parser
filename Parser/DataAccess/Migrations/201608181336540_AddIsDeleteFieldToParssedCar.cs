namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsDeleteFieldToParssedCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParssedCars", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParssedCars", "IsDeleted");
        }
    }
}
