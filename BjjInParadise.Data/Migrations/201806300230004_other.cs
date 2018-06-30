namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class other : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CampRoomOption", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CampRoomOption", "ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CampRoomOption", "ModifiedDate");
            DropColumn("dbo.CampRoomOption", "CreatedDate");
        }
    }
}
