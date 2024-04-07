using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("../UsersPage/UsersHome");
                }
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("../AdminsPage/ManageUsers");
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

            var usersRole = _db.UserRole.Where(model => model.UserID == user.userID).Select(model => model.RoleID).FirstOrDefault();

            if(usersRole == 2)
            {
                return RedirectToAction("../UsersPage/ChooseMood");
            }

            if(usersRole == 1)
            {
                return RedirectToAction("../AdminsPage/ManageUsers");
            }

            return View();
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
            var profilePicture = Request.Files.Get("profilePic");

            if (profilePicture != null && profilePicture.ContentLength > 0)
            {
                using (var binaryReader = new BinaryReader(profilePicture.InputStream))
                {
                    u.ProfilePicture = binaryReader.ReadBytes(profilePicture.ContentLength);
                }
            }

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

                    var addNewUserWithRole = new UserRole();

                    addNewUserWithRole.UserID = u.userID;
                    addNewUserWithRole.RoleID = 2;

                    _userRoleRepo.Create(addNewUserWithRole);

                    ViewBag.RegisterSuccessAlertMsg = "Successfully registered, try to log in with your new account";
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