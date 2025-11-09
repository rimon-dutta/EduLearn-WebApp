namespace WAPP_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flashcards", "DeckId", c => c.Int(nullable: false));
            CreateIndex("dbo.Flashcards", "DeckId");
            AddForeignKey("dbo.Flashcards", "DeckId", "dbo.FlashcardDecks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Flashcards", "DeckId", "dbo.FlashcardDecks");
            DropIndex("dbo.Flashcards", new[] { "DeckId" });
            DropColumn("dbo.Flashcards", "DeckId");
        }
    }
}
