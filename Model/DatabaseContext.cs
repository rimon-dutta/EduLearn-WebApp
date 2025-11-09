using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WAPP_Assignment.Model
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Flashcard> Flashcards { get; set; }

        public DbSet<FlashcardDeck> FlashcardDecks { get; set; }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flashcard>()
                .HasRequired(f => f.User)
                .WithMany(u => u.Flashcards)
                .HasForeignKey(f => f.UserId)
                .WillCascadeOnDelete(false);

            // Disable cascade delete from FlashcardDeck -> User
            modelBuilder.Entity<FlashcardDeck>()
                .HasRequired(d => d.User)
                .WithMany(u => u.FlashcardDecks)
                .HasForeignKey(d => d.UserId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}