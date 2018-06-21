namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        AspNetUserId = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipCode = c.String(),
                        HomeGym = c.String(),
                        Country = c.String(),
                        PhoneNumber = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.BookingParticipant",
                c => new
                    {
                        BookingParticipantId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        ParticipantId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingParticipantId)
                .ForeignKey("dbo.Booking", t => t.BookingId, cascadeDelete: true)
                .Index(t => t.BookingId);
            
            CreateTable(
                "dbo.Booking",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CampId = c.Int(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        AmountPaid = c.Decimal(precision: 18, scale: 2),
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
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CampId);
            
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
            DropForeignKey("dbo.BookingParticipant", "BookingId", "dbo.Booking");
            DropForeignKey("dbo.Booking", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Booking", "CampId", "dbo.Camp");
            DropIndex("dbo.Participant", new[] { "UserId" });
            DropIndex("dbo.Booking", new[] { "CampId" });
            DropIndex("dbo.Booking", new[] { "UserId" });
            DropIndex("dbo.BookingParticipant", new[] { "BookingId" });
            DropTable("dbo.Participant");
            DropTable("dbo.Camp");
            DropTable("dbo.Booking");
            DropTable("dbo.BookingParticipant");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
