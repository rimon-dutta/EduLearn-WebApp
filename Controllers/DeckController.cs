using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using WAPP_Assignment.Model;

namespace WAPP_Assignment.Controllers
{
    public class DeckController : Controller
    {
        private readonly DatabaseContext _context;

        public DeckController() : this(new DatabaseContext())
        {
        }

        public DeckController(DatabaseContext context)
        {
            _context = context;
        }

        public ActionResult Deck(int id)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var userId = (int)Session["userId"];
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var deck = _context.FlashcardDecks
                .Include(d => d.Flashcards)
                .FirstOrDefault(d => d.Id == id && d.UserId == userId);

            if (deck == null)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

            ViewBag.Deck = deck;
            ViewBag.Flashcards = (List<Flashcard>)deck.Flashcards;
            ViewBag.LearningStreak = user.learningStreak;
            return View();
        }

        public async Task<ActionResult> Learn(int id, int index = 1, bool revealed = false)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var userId = int.Parse(Session["userId"].ToString());
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var flashcardQuery = _context.Flashcards.AsQueryable();
            flashcardQuery = flashcardQuery.Where(f => f.UserId == userId && f.DeckId == id);
            var flashcards = await flashcardQuery.OrderBy(f => f.Id).ToListAsync();

            ViewBag.Flashcard = flashcards[index - 1];
            ViewBag.index = index;
            ViewBag.revealed = revealed;
            ViewBag.flashcardsAmount = flashcards.Count;
            ViewBag.DeckId = id;
            ViewBag.LearningStreak = user.learningStreak;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateDeckTitle(int id, string deckTitle)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (deckTitle.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Deck title cannot be empty.";
                ViewBag.error = true;
                return RedirectToAction("Deck", new { id = id });
            }

            var deck = _context.FlashcardDecks.Find(id);
            if (deck == null)
            {
                ViewBag.errorMessage = "Deck not found.";
                ViewBag.error = true;
                return RedirectToAction("Deck", new { id = id });
            }

            deck.Title = deckTitle;
            _context.SaveChanges();
            return RedirectToAction("Deck", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDeck(int id)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var deck = _context.FlashcardDecks.Find(id);
            if (deck == null)
            {
                ViewBag.errorMessage = "Deck not found.";
                ViewBag.error = true;
                return RedirectToAction("Dashboard", "Dashboard");
            }

            _context.FlashcardDecks.Remove(deck);
            _context.SaveChanges();
            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewFlashcard(int id, string question, string answer)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (question.IsNullOrWhiteSpace() || answer.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Question and Answer cannot be empty.";
                ViewBag.error = true;
                return RedirectToAction("Deck", new { id = id });
            }

            var flashcard = new Flashcard()
            {
                Question = question,
                Answer = answer,
                UserId = int.Parse(Session["userId"].ToString()),
                DeckId = id
            };
            _context.Flashcards.Add(flashcard);
            _context.SaveChanges();
            return RedirectToAction("Deck", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFlashcard(int id, string question, string answer, int deckId)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (question.IsNullOrWhiteSpace() || answer.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Question and Answer cannot be empty.";
                ViewBag.error = true;
                return RedirectToAction("Deck", new { id = deckId });
            }

            var flashcard = _context.Flashcards.Find(id);
            if (flashcard == null)
            {
                ViewBag.errorMessage = "Flashcard not found.";
                ViewBag.error = true;
                return RedirectToAction("Deck", new { id = deckId });
            }

            flashcard.Question = question;
            flashcard.Answer = answer;
            _context.SaveChanges();
            return RedirectToAction("Deck", new { id = deckId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFlashcard(int id, int deckId)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var flashcard = _context.Flashcards.Find(id);
            if (flashcard == null)
            {
                ViewBag.errorMessage = "Flashcard not found.";
                ViewBag.error = true;
                return RedirectToAction("Deck", new { id = deckId });
            }

            _context.Flashcards.Remove(flashcard);
            _context.SaveChanges();
            return RedirectToAction("Deck", new { id = deckId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishLearningSession(int deckId)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            var userId = int.Parse(Session["userId"].ToString());
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (user.lastLearned.Date.AddDays(1) == System.DateTime.Now.Date)
            {
                user.learningStreak += 1;
            }
            
            _context.SaveChanges();
            return RedirectToAction("Deck", new { id = deckId });
        }
    }
}