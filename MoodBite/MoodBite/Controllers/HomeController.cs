using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                    user = (User)_db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
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
        public ActionResult ViewProfile(User user, HttpPostedFileBase profilePic, string oldPwdInp, string dob)
        {
            var existingUser = _db.User.Where(model => model.userID == user.userID).FirstOrDefault();
            User u = new User();
            if (existingUser.Password == oldPwdInp)
            {
                if ((user != null && User.Identity.IsAuthenticated) && !string.IsNullOrEmpty(oldPwdInp) && !string.IsNullOrEmpty(dob) && !string.IsNullOrEmpty(user.Password))
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
                        if (profilePic == null)
                        {
                            u = user;
                            u.Email = existingUser.Email;
                            u.EmailConfirmed = existingUser.EmailConfirmed;
                            u.EmailConfirmationToken = existingUser.EmailConfirmationToken;
                            u.BirthDate = dateOfBirthModel;
                            u.ProfilePicturePath = existingUser.ProfilePicturePath;

                            try
                            {
                                _userRepo.Update(user.userID, u);
                                return Json(new { success = true, message = "Profile updated successfully" });
                            }
                            catch (Exception)
                            {
                                return Json(new { success = false, message = "An error has occured" });
                                throw;
                            }
                        } else
                        {
                            var profilePicture = Request.Files.Get("profilePic");

                            if (profilePicture != null && profilePicture.ContentLength > 0)
                            {
                                string fileName = Path.GetFileName(profilePicture.FileName);
                                string uniqueFileName = GetUniqueFileName("~/Content/UsersProfileImages/", fileName);
                                string filePath = Path.Combine(Server.MapPath("~/Content/UsersProfileImages/"), uniqueFileName);

                                // Save the file to the specified path
                                profilePicture.SaveAs(filePath);

                                // Store the file path in the database
                                u.ProfilePicturePath = "~/Content/UsersProfileImages/" + uniqueFileName;

                                u = user;
                                u.Email = existingUser.Email;
                                u.EmailConfirmed = existingUser.EmailConfirmed;
                                u.EmailConfirmationToken = existingUser.EmailConfirmationToken;
                                u.BirthDate = dateOfBirthModel;
                                try
                                {
                                    _userRepo.Update(user.userID, u);
                                    return Json(new { success = true, message = "Profile updated successfully" });
                                }
                                catch (Exception)
                                {
                                    return Json(new { success = false, message = "An error has occured" });
                                }
                                
                            }
                            else
                            {
                                return Json(new { success = false, message = "An error has occured" });
                            }
                        }
                    } else
                    {
                        return Json(new { success = true, message = "Invalid date format" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "An error has occured, please fill empty fields" });
                }
            } else
            {
                return Json(new { success = false, message = "Enter your correct old password!" });
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

                                ////use this when deployed
                                //string body = $"Please confirm your email address by clicking <a href=\"{confirmationLink}\">here</a>";

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
                                        smtp.Credentials = new NetworkCredential(from, password);
                                        smtp.EnableSsl = true;
                                        smtp.Send(message);
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