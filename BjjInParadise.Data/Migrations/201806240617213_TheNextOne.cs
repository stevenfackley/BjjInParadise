namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TheNextOne : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Camp", "HtmlPageText", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "AspNetUserId", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "Street", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "City", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "State", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "ZipCode", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "HomeGym", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "Country", c => c.String(nullable: false));
            AlterColumn("dbo.ApplicationUsers", "PhoneNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationUsers", "PhoneNumber", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "Country", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "HomeGym", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "ZipCode", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "State", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "City", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "Street", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "LastName", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "FirstName", c => c.String());
            AlterColumn("dbo.ApplicationUsers", "AspNetUserId", c => c.String());
            DropColumn("dbo.Camp", "HtmlPageText");
        }
    }
}
