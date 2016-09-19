namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableAdvertCarPrice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdvertCarPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        AdvertCarId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AdvertCars", t => t.AdvertCarId, cascadeDelete: true)
                .Index(t => t.AdvertCarId);
            
            AddColumn("dbo.AdvertCars", "Url", c => c.String());
            AddColumn("dbo.MainConfigurations", "DealerId", c => c.Int());
            CreateIndex("dbo.MainConfigurations", "DealerId");
            AddForeignKey("dbo.MainConfigurations", "DealerId", "dbo.Dealers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvertCarPrices", "AdvertCarId", "dbo.AdvertCars");
            DropForeignKey("dbo.MainConfigurations", "DealerId", "dbo.Dealers");
            DropIndex("dbo.MainConfigurations", new[] { "DealerId" });
            DropIndex("dbo.AdvertCarPrices", new[] { "AdvertCarId" });
            DropColumn("dbo.MainConfigurations", "DealerId");
            DropColumn("dbo.AdvertCars", "Url");
            DropTable("dbo.AdvertCarPrices");
        }
    }
}
