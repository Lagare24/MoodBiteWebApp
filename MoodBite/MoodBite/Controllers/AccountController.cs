using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MoodBite.Controllers
{
    [HandleError]
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
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
                    return RedirectToAction("../AdminsPage/Index");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User u, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var user = _userRepo.Table.Where(model => (model.Username == u.Username && model.Password == u.Password) && model.EmailConfirmed == true).FirstOrDefault();

                if (user != null)
                {
                    var isPremium = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();
                    FormsAuthentication.SetAuthCookie(u.Username, false);
                    ViewBag.LoginSuccess = true;
                    Session["User"] = user;
                    Session["IsPremium"] = isPremium;

                    var usersRole = _db.UserRole.Where(model => model.UserID == user.userID).Select(model => model.RoleID).FirstOrDefault();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        //returnUrl = returnUrl.Replace("/UsersPage", "");
                        Session["returnUrl"] = returnUrl;
                        return RedirectToAction(".."+returnUrl);
                    }
                    if (usersRole == 2)
                    {
                        return RedirectToAction("../UsersPage/ChooseMood");
                    }

                    if (usersRole == 1)
                    {
                        return RedirectToAction("../AdminsPage/Index");
                    }
                }
                else
                {
                    try
                    {
                        if (_userRepo.Table.Where(model => model.Username == u.Username).FirstOrDefault().Password != u.Password)
                        {
                            ModelState.AddModelError("", "Invalid password");
                            ViewBag.LoginSuccess = false;
                            return View();
                        }
                        else
                        {
                            ModelState.AddModelError("", "User not exist");
                            ViewBag.LoginSuccess = false;
                            return View();
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "User not exist");
                        ViewBag.LoginSuccess = false;
                        return View();
                    }
                }
            }
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("../AdminsPage/Index");
            }
            if (User.IsInRole("User"))
            {
                return RedirectToAction("../UsersPage/UsersHome");
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
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("../AdminsPage/Index");
                }
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("../UsersPage/UsersHome");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Register(User u, string confirmPasswordInp, string bod)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var existingUser = _db.User.Where(model => (model.Email == u.Email || model.Username == u.Username) && model.EmailConfirmed == true).FirstOrDefault();

                if (u == null || string.IsNullOrEmpty(confirmPasswordInp) || string.IsNullOrEmpty(bod))
                {
                    ModelState.AddModelError("", "Please fill in all the fields.");
                    return Json(new { success = false, msg = "Please fill in all the fields." });
                }

                if (!IsValidEmail(u.Email))
                {
                    ModelState.AddModelError("", "Invalid email format.");
                    return Json(new { success = false, msg = "Invalid email format." });
                }

                if (existingUser != null)
                {
                    if (existingUser.Email == u.Email)
                    {
                        ModelState.AddModelError("", "Email is already taken!");
                        return Json(new { success = false, msg = "Email is already taken!" });
                    }
                    if (existingUser.Username == u.Username)
                    {
                        ModelState.AddModelError("", "Username is already taken!");
                        return Json(new { success = false, msg = "Email is already taken!" });
                    }
                    return Json(new { success = false, msg = "Error in your email." });
                }
                else
                {
                    var profilePicture = Request.Files.Get("profilePic");

                    if (profilePicture != null && profilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(profilePicture.FileName);
                        string uniqueFileName = GetUniqueFileName("~/Content/UsersProfileImages/", fileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/UsersProfileImages/"), uniqueFileName);

                        profilePicture.SaveAs(filePath);

                        u.ProfilePicturePath = "~/Content/UsersProfileImages/" + uniqueFileName;
                    } else
                    {
                        if (u.Gender != null)
                        {
                            if (u.Gender == "Male")
                            {
                                u.ProfilePicturePath = "~/Content/UsersProfileImages/noprofile-boy.png";
                            } else
                            {
                                u.ProfilePicturePath = "~/Content/UsersProfileImages/noprofile-girl.jpg";
                            }
                        } else
                        {
                            ModelState.AddModelError("", "Please select your gender.");
                            return Json(new { success = false, msg = "Please select your gender." });
                        }
                    }
                    if (u.Password.Length < 8)
                    {
                        ModelState.AddModelError("", "Password must be at least 8 characters long.");
                        return Json(new { success = false, msg = "Password must be at least 8 characters long." });
                    }

                    if (!Regex.IsMatch(u.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).*$"))
                    {
                        ModelState.AddModelError("", "Password must contain at least one lowercase letter, one uppercase letter, and one symbol.");
                        return Json(new { success = false, msg = "Password must contain at least one lowercase letter, one uppercase letter, and one symbol." });
                    }

                    if (u.Password != confirmPasswordInp)
                    {
                        ModelState.AddModelError("", "Password not matched");
                        return Json(new { success = false, msg = "Password not matched" });
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
                        if (u.Gender != null)
                        {
                            u.EmailConfirmed = false;
                            try
                            {
                                string token = Guid.NewGuid().ToString().Substring(0, 6);
                                u.EmailConfirmationToken = token;
                                _userRepo.Create(u);

                                try
                                {
                                    SendConfirmationEmail(u);
                                }
                                catch (Exception)
                                {
                                    return Json(new { success = false, msg = "Invalid Recipient" });
                                }
                            }
                            catch (Exception)
                            {
                                return Json(new { success = false, msg = "An error has occured." });
                                throw;
                            }
                            Session["User"] = u;
                            return Json(new { success = true });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please select your gender.");
                            return Json(new { success = false, msg = "Please select your gender." });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Input for Birth of Date.");
                        return Json(new { success = false, msg = "Invalid Input for Birth of Date." });
                    }
                }
            } else
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("../AdminsPage/Index");
                }
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("../UsersPage/UsersHome");
                }
                return View();
            }
        }

        private void SendConfirmationEmail(User user)
        {
            string email = ConfigurationManager.AppSettings["Email"];
            string password = "cmobmopkqmvqdriu";
            string noreplyEmail = "no-reply@moodbite.com";

            string confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = user.EmailConfirmationToken }, Request.Url.Scheme);

            string subject = "Confirm your email address";

            ////use this when deployed
            //string body = $"Please confirm your email address by clicking <a href=\"{confirmationLink}\">here</a>";

            string body = $"Please confirm your email by entering the following verification code: {user.EmailConfirmationToken}";

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(noreplyEmail);
                message.To.Add(user.Email);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(email, password);
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
            }
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail()
        {
            if (Session["User"] != null)
            {
                var user = Session["User"] as User;
                return View();
            } else
            {
                return RedirectToAction("Register");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Error");
            }

            var user = _db.User.Where(model => model.EmailConfirmationToken == token).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid code");
                return RedirectToAction("ConfirmEmail");
            } else
            {
                user.EmailConfirmed = true;
                _userRepo.Update(user.userID, user);

                var addNewUserWithRole = new UserRole();
                addNewUserWithRole.UserID = user.userID;
                addNewUserWithRole.RoleID = 2;
                _userRoleRepo.Create(addNewUserWithRole);
                return RedirectToAction("LogIn");
            }
        }

        public ActionResult SubscribePremium(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubscribePremium()
        {
            return View();
        }
    }
}