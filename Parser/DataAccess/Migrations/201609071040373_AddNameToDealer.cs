namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToDealer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dealers", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dealers", "Name");
        }
    }
}
