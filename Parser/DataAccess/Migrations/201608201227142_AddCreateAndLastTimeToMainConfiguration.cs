namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreateAndLastTimeToMainConfiguration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MainConfigurations", "CreateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.MainConfigurations", "LastTimeUpdate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MainConfigurations", "LastTimeUpdate");
            DropColumn("dbo.MainConfigurations", "CreateTime");
        }
    }
}
