namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageSrcToAdvertCar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdvertCars", "ImageSrc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdvertCars", "ImageSrc");
        }
    }
}
