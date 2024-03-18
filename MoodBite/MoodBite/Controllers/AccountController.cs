using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MoodBite.Controllers
{
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        public ActionResult LogIn()
        {
            if (IsUserAuthenticated())
            {
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("../UsersPage/UsersHome");
                }
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("../UsersPage/UsersHome");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User u)
        {
            var user = _userRepo.Table.Where(model => model.Username == u.Username && model.Password == u.Password).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError("", "User not exist");
                return View();
            }
            else if (!user.Password.Equals(u.Password))
            {
                ModelState.AddModelError("", "Invalid password");
                return View();
            }

            FormsAuthentication.SetAuthCookie(u.Username, false);

            return RedirectToAction("../Home/InputMood");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User u)
        {
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("../Home/Index");
        }
    }
}