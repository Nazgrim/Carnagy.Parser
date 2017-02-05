namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocationsFieldsToDealer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dealers", "CityName", c => c.String());
            AddColumn("dbo.Dealers", "Province", c => c.String());
            AddColumn("dbo.Dealers", "Adress", c => c.String());
            AddColumn("dbo.Dealers", "ZipCode", c => c.String());
            AddColumn("dbo.Dealers", "Phone", c => c.String());
            AddColumn("dbo.Dealers", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Dealers", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dealers", "Longitude");
            DropColumn("dbo.Dealers", "Latitude");
            DropColumn("dbo.Dealers", "Phone");
            DropColumn("dbo.Dealers", "ZipCode");
            DropColumn("dbo.Dealers", "Adress");
            DropColumn("dbo.Dealers", "Province");
            DropColumn("dbo.Dealers", "CityName");
        }
    }
}
