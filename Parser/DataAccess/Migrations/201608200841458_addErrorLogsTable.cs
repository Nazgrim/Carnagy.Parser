namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addErrorLogsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        MainConfigurationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MainConfigurations", t => t.MainConfigurationId, cascadeDelete: true)
                .Index(t => t.MainConfigurationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ErrorLogs", "MainConfigurationId", "dbo.MainConfigurations");
            DropIndex("dbo.ErrorLogs", new[] { "MainConfigurationId" });
            DropTable("dbo.ErrorLogs");
        }
    }
}
