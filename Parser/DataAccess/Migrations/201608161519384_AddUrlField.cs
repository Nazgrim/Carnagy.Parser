namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUrlField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fields", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.ParssedCars", "Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ParssedCars", "Url");
            DropColumn("dbo.Fields", "IsDefault");
        }
    }
}
