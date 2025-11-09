namespace WAPP_Assignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixForeignKeys2 : DbMigration
    {
        public override void Up()
        {
            // remove existing FKs and indexes (if present)
            DropForeignKey("dbo.FlashcardDecks", "UserId", "dbo.Users");
            DropForeignKey("dbo.Flashcards", "UserId", "dbo.Users");
            DropIndex("dbo.FlashcardDecks", new[] { "UserId" });
            DropIndex("dbo.Flashcards", new[] { "UserId" });

            // recreate indexes and FKs without cascade delete
            CreateIndex("dbo.FlashcardDecks", "UserId");
            CreateIndex("dbo.Flashcards", "UserId");
            AddForeignKey("dbo.FlashcardDecks", "UserId", "dbo.Users", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Flashcards", "UserId", "dbo.Users", "Id", cascadeDelete: false);
        }

        public override void Down()
        {
            // revert: remove non-cascade FKs and indexes, then recreate with cascade
            DropForeignKey("dbo.Flashcards", "UserId", "dbo.Users");
            DropForeignKey("dbo.FlashcardDecks", "UserId", "dbo.Users");
            DropIndex("dbo.Flashcards", new[] { "UserId" });
            DropIndex("dbo.FlashcardDecks", new[] { "UserId" });

            CreateIndex("dbo.Flashcards", "UserId");
            CreateIndex("dbo.FlashcardDecks", "UserId");
            AddForeignKey("dbo.Flashcards", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FlashcardDecks", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
