using MoodBite.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index()
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
            var testimonialItems = _db.Testimonials.ToList();
            return View(testimonialItems);
        }

        [HttpPost]
        public ActionResult SubmitTestimony(string name, string email, string profession, string message)
        {
            var existingTestimonial = _db.Testimonials.Where(model => model.tEmail == email).FirstOrDefault();
            if (existingTestimonial == null)
            {
                var testimony = new Testimonials();
                testimony.tName = name;
                testimony.tEmail = email;
                testimony.tProfession = profession;
                testimony.tMsg = message;

                try
                {
                    _testimonyRepo.Create(testimony);
                    return Json(new { msg = "Feedback submitted successfully" });
                }
                catch (Exception)
                {
                    return Json(new { msg = "An error occurred while submitting the testimony" });
                    throw;
                }
            } else
            {
                return Json(new { msg="Can only submit feedback once per email" });
            }
        }

        public ActionResult ViewProfile()
        {
            User user = new User();
            if (Session["User"] != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var u = Session["User"] as User;
                    user = (User)_db.User.Where(model => model.userID == u.userID).FirstOrDefault();
                    return View(user);
                }
                else
                {
                    return RedirectToAction("../Account/LogOut");
                }
            }
            else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        public ActionResult ViewProfile(User user, string oldPwdInp, string dob)
        {
            if (User.Identity.IsAuthenticated)
            {
                var existingUser = _db.User.Where(model => model.userID == user.userID).FirstOrDefault();
                var profilePicture = Request.Files.Get("profilePic");
                if (!string.IsNullOrEmpty(dob))
                {
                    string[] dateInitInp = dob.Split('/');
                    string[] formatDateInitInp = new string[dateInitInp.Length];
                    formatDateInitInp[0] = dateInitInp[1];
                    formatDateInitInp[1] = dateInitInp[0];
                    formatDateInitInp[2] = dateInitInp[2];
                    string formattedDate = string.Join("-", formatDateInitInp);
                    DateTime dateOfBirthModel = new DateTime();
                    if (DateTime.TryParseExact(formattedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthModel))
                    {
                        user.BirthDate = dateOfBirthModel;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid date format" });
                    }

                    if (profilePicture != null && profilePicture.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(profilePicture.FileName);
                        string uniqueFileName = GetUniqueFileName("~/Content/UsersProfileImages/", fileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/UsersProfileImages/"), uniqueFileName);

                        profilePicture.SaveAs(filePath);

                        user.ProfilePicturePath = "~/Content/UsersProfileImages/" + uniqueFileName;
                    }
                    else
                    {
                        user.ProfilePicturePath = existingUser.ProfilePicturePath;
                    }

                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        if (oldPwdInp == existingUser.Password)
                        {
                            if (user.Password.Length < 8)
                            {
                                return Json(new { success = false, message = "Password must be at least 8 characters long." });
                            }
                            if (!Regex.IsMatch(user.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).*$"))
                            {
                                return Json(new { success = false, message = "Password must contain at least one lowercase letter, one uppercase letter, and one symbol." });
                            }
                            user.Password = user.Password;
                            if (_userRepo.Update(user.userID, user) == ErrorCode.Success)
                            {
                                Session["User"] = user;
                                return Json(new { success = true, message = "Profile updated successfully!" });
                            }
                            else
                            {
                                return Json(new { success = false, message = "An error has occured." });
                            }
                        }
                        else if (string.IsNullOrEmpty(oldPwdInp))
                        {
                            return Json(new { success = false, message = "Please enter your old password" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Incorrect old password" });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(oldPwdInp))
                        {
                            return Json(new { success = false, message = "Fill in your new password" });
                        }
                        user.Password = existingUser.Password;
                        if (_userRepo.Update(user.userID, user) == ErrorCode.Success)
                        {
                            Session["User"] = user;
                            return Json(new { success = true, message = "Profile updated successfully!" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "An error has occured." });
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Date of birth is empty." });
                }
            } else
            {
                return Json(new { success = false, message = "Session timeout." });
            }
        }

        public ActionResult BuyPremium(int id)
        {
            var prem = _db.Premium.Where(model => model.PremiumID == id).FirstOrDefault();
            TempData["prem"] = prem;
            return View(prem);
        }

        [HttpPost]
        public ActionResult BuyPremium(string phoneNumber, string email)
        {
            if (phoneNumber.Length <= 0 || phoneNumber.Length >= 11)
            {
                return Json(new { success = false, msg = "An error has occurred. Invalid phone number" });
            }
            if (TempData["prem"] != null)
            {
                var user = _db.User.Where(model => model.Email == email).FirstOrDefault();
                if (user != null)
                {
                    var existingSubscription = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();

                    if (existingSubscription == null)
                    {
                        var prem = TempData["prem"] as Premium;
                        var userPremium = new UserPremium();
                        var transaction = new BuyPremiumTransaction();

                        transaction.customerEmail = email;
                        transaction.premiumTypeID = prem.PremiumID;
                        transaction.amoountPaid = 999;
                        transaction.phoneNumber = phoneNumber;

                        try
                        {
                            _buyPremTransactionRepo.Create(transaction);
                            var transactId = transaction.transactionID;

                            userPremium.UserID = user.userID;
                            userPremium.PremiumID = prem.PremiumID;
                            userPremium.DateSubscribed = DateTime.Today;

                            try
                            {
                                _userPremium.Create(userPremium);

                                string from = ConfigurationManager.AppSettings["Email"];
                                string password = "cmobmopkqmvqdriu";
                                string noreplyEmail = "no-reply@moodbite.com";


                                string subject = "Account Upgrade Reciept";

                                string body = $"<p>Thank you for upgrading your account to {prem.PremiumType}.</p>"
                + $"<p>Date of Subscription: {DateTime.Today:d}</p>"
                + $"<p>Your subscription will expire on: {DateTime.Today.AddDays(Convert.ToDouble(prem.Duration)):d}</p>"
                + "<p>Enjoy the enhanced features and benefits that come with your premium account.</p>";

                                using (MailMessage message = new MailMessage())
                                {
                                    message.From = new MailAddress(noreplyEmail);
                                    message.To.Add(user.Email);
                                    message.Subject = subject;
                                    message.Body = body;
                                    message.IsBodyHtml = true;

                                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                    {
                                        
                                        try
                                        {
                                            smtp.Credentials = new NetworkCredential(from, password);
                                            smtp.EnableSsl = true;
                                            smtp.Send(message);
                                        }
                                        catch (Exception)
                                        {
                                            return Json(new { success = false, msg = "Invalid Recipient" });
                                        }
                                    }
                                }
                                return Json(new { success = true, msg = "Subscribed successfully, check email for reciepts" });
                            }
                            catch (Exception)
                            {
                                _buyPremTransactionRepo.Delete(transactId);
                                throw;
                            }
                        }
                        catch (Exception)
                        {
                            return Json(new { success = false, msg = "An error has occurred. Payment has not been completed." });
                            throw;
                        }
                    } else
                    {
                        return Json(new { success = false, msg = "This account has an ongoing subscription!" });
                    }
                } else
                {
                    return Json(new { success = false, msg = "An error has occurred. Payment has not been completed." });
                }
            } else
            {
                return Json(new { success = false, msg = "An error has occurred. Payment has not been completed." });
            }
        }
    }
}