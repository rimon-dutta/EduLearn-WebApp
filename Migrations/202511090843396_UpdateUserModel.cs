namespace WAPP_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "lastLearned", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "learningStreak", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "learningStreak");
            DropColumn("dbo.Users", "lastLearned");
        }
    }
}
