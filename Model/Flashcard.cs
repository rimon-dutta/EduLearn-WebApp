using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAPP_Assignment.Model
{
    public class Flashcard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")] public int UserId { get; set; }

        [ForeignKey("Deck")] public int? DeckId { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }

        public virtual FlashcardDeck Deck { get; set; }

        public virtual User User { get; set; }
    }
}