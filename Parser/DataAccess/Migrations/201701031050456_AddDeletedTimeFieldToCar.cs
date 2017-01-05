namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeletedTimeFieldToCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "DeletedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "DeletedTime");
        }
    }
}
