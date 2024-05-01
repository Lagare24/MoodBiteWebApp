using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            if(User.Identity.IsAuthenticated)
            {
                if(User.IsInRole("User"))
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

                            _userRepo.Update(user.userID, u);
                            return Json(new { success = true, message = "Profile updated successfully" });
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
                            //var profilePicture = Request.Files.Get("profilePic");

                            //if (profilePicture != null && profilePicture.ContentLength > 0)
                            //{
                            //    using (var binaryReader = new BinaryReader(profilePicture.InputStream))
                            //    {
                            //        u = user;
                            //        u.Email = existingUser.Email;
                            //        u.EmailConfirmed = existingUser.EmailConfirmed;
                            //        u.EmailConfirmationToken = existingUser.EmailConfirmationToken;
                            //        u.BirthDate = dateOfBirthModel;
                            //        u.ProfilePicturePath = binaryReader.ReadBytes(profilePicture.ContentLength);

                            //        _userRepo.Update(user.userID, u);
                            //        return Json(new { success = true, message = "Profile updated successfully" });
                            //    }
                            //} else
                            //{
                            //    return Json(new { success = true, message = "An error has occured" });
                            //}
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
    }
}