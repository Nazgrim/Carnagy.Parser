namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsCreatedFieldInDealer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dealers", "IsCreated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dealers", "IsCreated");
        }
    }
}
