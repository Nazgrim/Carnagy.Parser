namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFiledIsCarDeletedToParsedCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParsedCars", "IsCarDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParsedCars", "IsCarDeleted");
        }
    }
}
