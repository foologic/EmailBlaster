namespace EmailBlaster.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Campaign",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Sender = c.String(nullable: false, maxLength: 80),
                        Subject = c.String(nullable: false, maxLength: 80),
                        HtmlBody = c.String(),
                        TextBody = c.String(),
                        DateScheduled = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateSent = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 80),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 80),
                        Active = c.Boolean(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        DateDeactivated = c.DateTime(),
                        Unsubscribed = c.Boolean(nullable: false),
                        Bounced = c.Boolean(nullable: false),
                        MarkedSpam = c.Boolean(nullable: false),
                        InvalidMailbox = c.Boolean(nullable: false),
                        Blocked = c.Boolean(nullable: false),
                        SuppressionReason = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Contact");
            DropTable("dbo.Campaign");
        }
    }
}
