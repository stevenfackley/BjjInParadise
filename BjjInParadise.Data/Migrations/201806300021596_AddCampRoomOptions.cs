namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCampRoomOptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CampRoomOption",
                c => new
                    {
                        CampRoomOptionId = c.Int(nullable: false, identity: true),
                        CampId = c.Int(nullable: false),
                        RoomType = c.String(nullable: false),
                        CostPerPerson = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CampRoomOptionId)
                .ForeignKey("dbo.Camp", t => t.CampId, cascadeDelete: true)
                .Index(t => t.CampId);
            
            AddColumn("dbo.Booking", "CampRoomOptionId", c => c.Int());
            CreateIndex("dbo.Booking", "CampRoomOptionId");
            AddForeignKey("dbo.Booking", "CampRoomOptionId", "dbo.CampRoomOption", "CampRoomOptionId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Booking", "CampRoomOptionId", "dbo.CampRoomOption");
            DropForeignKey("dbo.CampRoomOption", "CampId", "dbo.Camp");
            DropIndex("dbo.CampRoomOption", new[] { "CampId" });
            DropIndex("dbo.Booking", new[] { "CampRoomOptionId" });
            DropColumn("dbo.Booking", "CampRoomOptionId");
            DropTable("dbo.CampRoomOption");
        }
    }
}
