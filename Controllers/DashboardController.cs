using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using WAPP_Assignment.Model;

namespace WAPP_Assignment.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DatabaseContext _context;

        public DashboardController() : this(new DatabaseContext())
        {
        }

        public DashboardController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Dashboard()
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

            if (user.lastLearned.Date.AddDays(1) < System.DateTime.Now.Date)
            {
                user.learningStreak += 1;
            }
            
            await _context.SaveChangesAsync();
            
            var flashcardQuery = _context.Flashcards.AsQueryable();
            flashcardQuery = flashcardQuery.Where(f => f.UserId == userId);
            var flashcards = await flashcardQuery.OrderBy(f => f.Id).ToListAsync();

            var flashcardDeckQuery = _context.FlashcardDecks.AsQueryable();
            flashcardDeckQuery = flashcardDeckQuery.Where(d => d.UserId == userId);
            var decks = await flashcardDeckQuery.OrderBy(d => d.Id).ToListAsync();

            ViewBag.Decks = decks;
            ViewBag.Flashcards = flashcards;
            ViewBag.LearningStreak = user.learningStreak;
            return View();
        }

        public async Task<ActionResult> Learn(int index = 1, bool revealed = false)
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
            flashcardQuery = flashcardQuery.Where(f => f.UserId == userId);
            var flashcards = await flashcardQuery.OrderBy(f => f.Id).ToListAsync();

            ViewBag.Flashcard = flashcards[index - 1];
            ViewBag.index = index;
            ViewBag.revealed = revealed;
            ViewBag.flashcardsAmount = flashcards.Count;
            ViewBag.LearningStreak = user.learningStreak;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFolder(string deckTitle)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (deckTitle.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Deck title cannot be empty.";
                ViewBag.error = true;
                return View("Dashboard");
            }

            var deck = new FlashcardDeck()
            {
                Title = deckTitle,
                UserId = int.Parse(Session["userId"].ToString())
            };
            _context.FlashcardDecks.Add(deck);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewFlashcard(string question, string answer)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (question.IsNullOrWhiteSpace() || answer.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Question and Answer cannot be empty.";
                ViewBag.error = true;
                return View("Dashboard");
            }

            var flashcard = new Flashcard()
            {
                Question = question,
                Answer = answer,
                UserId = int.Parse(Session["userId"].ToString())
            };
            _context.Flashcards.Add(flashcard);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFlashcard(int id, string question, string answer)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (question.IsNullOrWhiteSpace() || answer.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Question and Answer cannot be empty.";
                ViewBag.error = true;
                return View("Dashboard");
            }

            var flashcard = _context.Flashcards.Find(id);
            if (flashcard == null)
            {
                ViewBag.errorMessage = "Flashcard not found.";
                ViewBag.error = true;
                return View("Dashboard");
            }

            flashcard.Question = question;
            flashcard.Answer = answer;
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFlashcard(int id)
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
                return View("Dashboard");
            }

            _context.Flashcards.Remove(flashcard);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishLearningSession()
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
            return RedirectToAction("Dashboard");
        }
    }
}