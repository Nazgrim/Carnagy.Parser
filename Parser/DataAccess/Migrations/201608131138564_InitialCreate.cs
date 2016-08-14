namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fields",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Xpath = c.String(),
                        MainConfigurationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MainConfigurations", t => t.MainConfigurationId, cascadeDelete: true)
                .Index(t => t.MainConfigurationId);
            
            CreateTable(
                "dbo.FieldValues",
                c => new
                    {
                        FieldId = c.Int(nullable: false),
                        ParssedCarId = c.Int(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => new { t.FieldId, t.ParssedCarId })
                .ForeignKey("dbo.Fields", t => t.FieldId, cascadeDelete: true)
                .ForeignKey("dbo.ParssedCars", t => t.ParssedCarId, cascadeDelete: true)
                .Index(t => t.FieldId)
                .Index(t => t.ParssedCarId);
            
            CreateTable(
                "dbo.ParssedCars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MainConfigurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SiteUrl = c.String(),
                        HoursPeriond = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fields", "MainConfigurationId", "dbo.MainConfigurations");
            DropForeignKey("dbo.FieldValues", "ParssedCarId", "dbo.ParssedCars");
            DropForeignKey("dbo.FieldValues", "FieldId", "dbo.Fields");
            DropIndex("dbo.FieldValues", new[] { "ParssedCarId" });
            DropIndex("dbo.FieldValues", new[] { "FieldId" });
            DropIndex("dbo.Fields", new[] { "MainConfigurationId" });
            DropTable("dbo.MainConfigurations");
            DropTable("dbo.ParssedCars");
            DropTable("dbo.FieldValues");
            DropTable("dbo.Fields");
        }
    }
}
