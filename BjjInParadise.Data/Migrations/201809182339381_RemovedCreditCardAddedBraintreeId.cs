namespace BjjInParadise.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedCreditCardAddedBraintreeId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Booking", "BrainTreeTransactionId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Booking", "BrainTreeTransactionId");
        }
    }
}
