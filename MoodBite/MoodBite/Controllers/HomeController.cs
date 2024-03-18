using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "User, Admin")]
        public ActionResult InputMood()
        {
            return View(_db.Mood.ToList());
        }

        [HttpPost]
        public ActionResult InputMood(FormCollection form)
        {
            string selectedMood = form["selectedMood"];
            if (!string.IsNullOrEmpty(selectedMood))
            {
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("../UsersPage/UsersHome", new { mood = selectedMood });
                }
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("../AdminsPage/AdminsHome", new { mood = selectedMood });
                }
            }
            return View();
        }

    }
}