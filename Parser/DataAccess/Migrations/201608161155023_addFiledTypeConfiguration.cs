namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFiledTypeConfiguration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fields", "ConfigurationType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fields", "ConfigurationType");
        }
    }
}
