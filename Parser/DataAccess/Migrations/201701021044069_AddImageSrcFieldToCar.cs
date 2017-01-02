namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageSrcFieldToCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cars", "ImageSrc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cars", "ImageSrc");
        }
    }
}
