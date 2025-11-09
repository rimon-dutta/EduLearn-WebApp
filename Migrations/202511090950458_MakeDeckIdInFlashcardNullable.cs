namespace WAPP_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeDeckIdInFlashcardNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Flashcards", "DeckId", "dbo.FlashcardDecks");
            DropIndex("dbo.Flashcards", new[] { "DeckId" });
            AlterColumn("dbo.Flashcards", "DeckId", c => c.Int());
            CreateIndex("dbo.Flashcards", "DeckId");
            AddForeignKey("dbo.Flashcards", "DeckId", "dbo.FlashcardDecks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Flashcards", "DeckId", "dbo.FlashcardDecks");
            DropIndex("dbo.Flashcards", new[] { "DeckId" });
            AlterColumn("dbo.Flashcards", "DeckId", c => c.Int(nullable: false));
            CreateIndex("dbo.Flashcards", "DeckId");
            AddForeignKey("dbo.Flashcards", "DeckId", "dbo.FlashcardDecks", "Id", cascadeDelete: true);
        }
    }
}
