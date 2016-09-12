namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedDiller : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                update dbo.Fields set Name = 'DealerName' where Name = 'DillerName';
                update dbo.Fields set Name = 'DealerLogo' where Name = 'DillerLogo';
                update dbo.Fields set Name = 'DealerPlace' where Name = 'DillerPlace'; 
            ");
        }
        
        public override void Down()
        {
            Sql(@"
                update dbo.Fields set Name = 'DillerName' where Name = 'DealerName';
                update dbo.Fields set Name = 'DillerLogo' where Name = 'DealerLogo';
                update dbo.Fields set Name = 'DillerPlace' where Name = 'DealerPlace'; 
            ");
        }
    }
}
