namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogoFieldInDealer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dealers", "Logo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dealers", "Logo");
        }
    }
}
