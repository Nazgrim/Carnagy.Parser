namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPriceToCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "Price");
        }
    }
}
