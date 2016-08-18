namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRelationBetweenParsedCarAndMainConfiguration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParssedCars", "MainConfigurationId", c => c.Int(nullable: false));
            CreateIndex("dbo.ParssedCars", "MainConfigurationId");
            AddForeignKey("dbo.ParssedCars", "MainConfigurationId", "dbo.MainConfigurations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParssedCars", "MainConfigurationId", "dbo.MainConfigurations");
            DropIndex("dbo.ParssedCars", new[] { "MainConfigurationId" });
            DropColumn("dbo.ParssedCars", "MainConfigurationId");
        }
    }
}
