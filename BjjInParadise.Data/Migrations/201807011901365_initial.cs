namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        AspNetUserId = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Street = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        ZipCode = c.String(nullable: false),
                        HomeGym = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Booking",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        BookingDate = c.DateTime(nullable: false),
                        AmountPaid = c.Decimal(precision: 18, scale: 2),
                        UserId = c.Int(nullable: false),
                        CampId = c.Int(nullable: false),
                        CampRoomOptionId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Camp", t => t.CampId, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CampId);
            
            CreateTable(
                "dbo.Camp",
                c => new
                    {
                        CampId = c.Int(nullable: false, identity: true),
                        CampName = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        HtmlPageText = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CampId);
            
            CreateTable(
                "dbo.CampRoomOption",
                c => new
                    {
                        CampRoomOptionId = c.Int(nullable: false, identity: true),
                        CampId = c.Int(nullable: false),
                        RoomType = c.String(nullable: false),
                        CostPerPerson = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpotsAvailable = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CampRoomOptionId)
                .ForeignKey("dbo.Camp", t => t.CampId, cascadeDelete: true)
                .Index(t => t.CampId);
            
            CreateTable(
                "dbo.Participant",
                c => new
                    {
                        ParticipantId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ParticipantId)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Participant", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Booking", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Booking", "CampId", "dbo.Camp");
            DropForeignKey("dbo.CampRoomOption", "CampId", "dbo.Camp");
            DropIndex("dbo.Participant", new[] { "UserId" });
            DropIndex("dbo.CampRoomOption", new[] { "CampId" });
            DropIndex("dbo.Booking", new[] { "CampId" });
            DropIndex("dbo.Booking", new[] { "UserId" });
            DropTable("dbo.Participant");
            DropTable("dbo.CampRoomOption");
            DropTable("dbo.Camp");
            DropTable("dbo.Booking");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
