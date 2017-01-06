namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMsrpFieldToCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "MsrpPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "MsrpPrice");
        }
    }
}
