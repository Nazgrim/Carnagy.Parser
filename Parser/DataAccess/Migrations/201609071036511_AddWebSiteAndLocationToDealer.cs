namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWebSiteAndLocationToDealer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dealers", "Location", c => c.String());
            AddColumn("dbo.Dealers", "WebSireUrl", c => c.String());
            AddColumn("dbo.Dealers", "WebSiteName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dealers", "WebSiteName");
            DropColumn("dbo.Dealers", "WebSireUrl");
            DropColumn("dbo.Dealers", "Location");
        }
    }
}
