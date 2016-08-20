namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPriceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Value = c.String(),
                        ParssedCarId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ParssedCars", t => t.ParssedCarId, cascadeDelete: true)
                .Index(t => t.ParssedCarId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prices", "ParssedCarId", "dbo.ParssedCars");
            DropIndex("dbo.Prices", new[] { "ParssedCarId" });
            DropTable("dbo.Prices");
        }
    }
}
