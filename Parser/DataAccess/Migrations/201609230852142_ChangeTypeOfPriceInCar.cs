namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeOfPriceInCar : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE dbo.Cars DROP CONSTRAINT DF__Cars__Price__412EB0B6");
            AlterColumn("dbo.Cars", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cars", "Price", c => c.Int(nullable: false));
        }
    }
}
