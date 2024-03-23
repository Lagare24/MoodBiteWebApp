using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
            try
            {
                if (_userRepo.Table.Where(model => model.Username == u.Username).FirstOrDefault().Password != u.Password)
                {
                    ModelState.AddModelError("", "Invalid password");
                    ViewBag.LoginSuccess = false;
                    return View();
                }
            }
            catch (NullReferenceException)
            {
                ModelState.AddModelError("", "User not exist");
                ViewBag.LoginSuccess = false;
                return View();
            }
            catch (TargetException)
            {
                ModelState.AddModelError("", "User not exist");
                ViewBag.LoginSuccess = false;
                return View();
            }

            FormsAuthentication.SetAuthCookie(u.Username, false);
            ViewBag.LoginSuccess = true;
            Session["User"] = user;

            //return RedirectToAction("Register");
            return RedirectToAction("../UsersPage/ChooseMood");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("LogIn");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User u, string confirmPasswordInp, string bod)
        {
            if (u.Password != confirmPasswordInp)
            {
                ModelState.AddModelError("", "Password not matched");
                return View();
            }

            string[] dateInitInp = bod.Split('/');
            string[] formatDateInitInp = new string[dateInitInp.Length];

            formatDateInitInp[0] = dateInitInp[1];
            formatDateInitInp[1] = dateInitInp[0];
            formatDateInitInp[2] = dateInitInp[2];

            string formattedDate = string.Join("-", formatDateInitInp);

            DateTime dateOfBirthModel = new DateTime();

            if (DateTime.TryParseExact(formattedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthModel))
            {
                u.BirthDate = dateOfBirthModel;
                if(u.Gender != null)
                {
                    _userRepo.Create(u);
                    return RedirectToAction("LogIn");
                } else
                {
                    ModelState.AddModelError("", "Please select your gender");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Input for Birth of Date");
                return View();
            }
        }
    }
}