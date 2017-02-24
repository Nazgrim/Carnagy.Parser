namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFiledIsUsedInCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "IsUsed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "IsUsed");
        }
    }
}
