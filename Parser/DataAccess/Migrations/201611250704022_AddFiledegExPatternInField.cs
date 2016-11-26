namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFiledegExPatternInField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Fields", "RegExPattern", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Fields", "RegExPattern");
        }
    }
}
