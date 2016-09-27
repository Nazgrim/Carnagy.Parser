namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusFieldInParsedCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParsedCars", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParsedCars", "Status");
        }
    }
}
