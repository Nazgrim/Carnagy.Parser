namespace DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameParssedCar : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ParssedCars", newName: "ParsedCars");
            RenameColumn(table: "dbo.FieldValues", name: "ParssedCarId", newName: "ParsedCarId");
            RenameColumn(table: "dbo.Prices", name: "ParssedCarId", newName: "ParsedCarId");
            RenameIndex(table: "dbo.FieldValues", name: "IX_ParssedCarId", newName: "IX_ParsedCarId");
            RenameIndex(table: "dbo.Prices", name: "IX_ParssedCarId", newName: "IX_ParsedCarId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Prices", name: "IX_ParsedCarId", newName: "IX_ParssedCarId");
            RenameIndex(table: "dbo.FieldValues", name: "IX_ParsedCarId", newName: "IX_ParssedCarId");
            RenameColumn(table: "dbo.Prices", name: "ParsedCarId", newName: "ParssedCarId");
            RenameColumn(table: "dbo.FieldValues", name: "ParsedCarId", newName: "ParssedCarId");
            RenameTable(name: "dbo.ParsedCars", newName: "ParssedCars");
        }
    }
}
