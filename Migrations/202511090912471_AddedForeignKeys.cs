namespace WAPP_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedForeignKeys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FlashcardDecks", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.FlashcardDecks", "UserId");
            CreateIndex("dbo.Flashcards", "UserId");
            AddForeignKey("dbo.Flashcards", "UserId", "dbo.Users", "Id", cascadeDelete: false);
            AddForeignKey("dbo.FlashcardDecks", "UserId", "dbo.Users", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FlashcardDecks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Flashcards", "UserId", "dbo.Users");
            DropIndex("dbo.Flashcards", new[] { "UserId" });
            DropIndex("dbo.FlashcardDecks", new[] { "UserId" });
            DropColumn("dbo.FlashcardDecks", "UserId");
        }
    }
}
