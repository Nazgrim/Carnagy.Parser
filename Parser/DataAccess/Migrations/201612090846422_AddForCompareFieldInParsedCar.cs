namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForCompareFieldInParsedCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParsedCars", "ForCompare", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParsedCars", "ForCompare");
        }
    }
}
