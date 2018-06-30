namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addspots : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CampRoomOption", "SpotsAvailable", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CampRoomOption", "SpotsAvailable");
        }
    }
}
