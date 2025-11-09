using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using WAPP_Assignment.Model;

namespace WAPP_Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        
        public HomeController() : this(new DatabaseContext()) {}
        
        public HomeController(DatabaseContext context)
        {
            _context = context;
        }
        
        public ActionResult Index()
        {
            if (Session["userId"] == null)
            {
                return View();
            }

            return View("IndexLoggedIn");
        }

        public ActionResult LogIn()
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }
            return View();
        }

        public ActionResult SignUp()
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAction(string username, string password)
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if (username.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Invalid login or password";
                ViewBag.error = true;
                return View("LogIn");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ViewBag.errorMessage = "Username does not exist.";
                ViewBag.error = true;
                return View("LogIn");
            }

            if (user.Password != password)
            {
                ViewBag.errorMessage = "Invalid login or password";
                ViewBag.error = true;
                return View("LogIn");
            }

            Session["userId"] = user.Id;
            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUpAction(string username, string password)
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (username.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
            {
                ViewBag.errorMessage = "Invalid login or password";
                ViewBag.error = true;
                return View("SignUp");
            }
            if (_context.Users.Any(u => u.Username == username))
            {
                ViewBag.errorMessage = "Username already exists.";
                ViewBag.error = true;
                return View("SignUp");
            }

            var user = new User
            {
                Username = username,
                Password = password,
                learningStreak = 0,
                lastLearned = DateTime.Today
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            
            Session["userId"] = user.Id;
            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session["userId"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}