namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Participants : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BookingParticipant", "BookingId", "dbo.Booking");
            DropIndex("dbo.BookingParticipant", new[] { "BookingId" });
            DropTable("dbo.BookingParticipant");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.BookingParticipantId);
            
            CreateIndex("dbo.BookingParticipant", "BookingId");
            AddForeignKey("dbo.BookingParticipant", "BookingId", "dbo.Booking", "BookingId", cascadeDelete: true);
        }
    }
}
