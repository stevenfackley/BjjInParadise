namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Login_Requirement_For_Booking : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Booking", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.Booking", new[] { "UserId" });
            AddColumn("dbo.Booking", "FirstName", c => c.String());
            AddColumn("dbo.Booking", "LastName", c => c.String());
            AddColumn("dbo.Booking", "EmailAddress", c => c.String());
            AlterColumn("dbo.Booking", "UserId", c => c.Int(true));
            CreateIndex("dbo.Booking", "UserId");
            AddForeignKey("dbo.Booking", "UserId", "dbo.ApplicationUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Booking", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.Booking", new[] { "UserId" });
            AlterColumn("dbo.Booking", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.Booking", "EmailAddress");
            DropColumn("dbo.Booking", "LastName");
            DropColumn("dbo.Booking", "FirstName");
            CreateIndex("dbo.Booking", "UserId");
            AddForeignKey("dbo.Booking", "UserId", "dbo.ApplicationUsers", "UserId", cascadeDelete: true);
        }
    }
}
