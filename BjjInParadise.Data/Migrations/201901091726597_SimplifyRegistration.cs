namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimplifyRegistration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ApplicationUsers", "Street");
            DropColumn("dbo.ApplicationUsers", "City");
            DropColumn("dbo.ApplicationUsers", "State");
            DropColumn("dbo.ApplicationUsers", "Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationUsers", "Country", c => c.String(nullable: false));
            AddColumn("dbo.ApplicationUsers", "State", c => c.String(nullable: false));
            AddColumn("dbo.ApplicationUsers", "City", c => c.String(nullable: false));
            AddColumn("dbo.ApplicationUsers", "Street", c => c.String(nullable: false));
        }
    }
}
