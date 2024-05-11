using MoodBite.Models.ManageUploads;
using MoodBite.Models.UserViewModel;
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
    [Authorize(Roles = "Admin")]
    public class AdminsPageController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview()
        {
            return View();
        }
        //Manage Users
        public ActionResult ManageUsers()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_db.vw_ManageUsers.ToList());
            }
            return RedirectToAction("../Account/Login");
        }
        public ActionResult ManageUploads()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_db.vw_ManageUploads.ToList());
            }
            return RedirectToAction("../Account/Login");
        }


        public ActionResult EditUser(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userView = new UserViewModel();
                userView.user = _db.User.Where(model => model.userID == id).FirstOrDefault();
                userView.userRole = _db.UserRole.Where(model => model.UserID == id).FirstOrDefault();
                userView.userRoleView = _db.vw_RoleView.Where(model => model.RoleID == userView.userRole.RoleID).FirstOrDefault();


                var user = _db.User.Where(model => model.userID == id).FirstOrDefault();
                Session["UserRole"] = _db.vw_RoleView.Where(model => model.RoleID == userView.userRole.RoleID).FirstOrDefault();
                
                return View(user);
            }
            return RedirectToAction("../Account/Login");
        }

        [HttpPost]
        public ActionResult EditUser(User u, HttpPostedFileBase profilePic, string pwdinp, string dob, string rolename)
        {
            if(Session["UserRole"] != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    try
                    {
                        int rolenameInt = Convert.ToInt32(rolename);

                        var userRoleSession = Session["UserRole"] as vw_RoleView;
                        var existingUser = _db.User.Where(model => model.userID == u.userID).FirstOrDefault();
                        var user = new User();
                        var userRole = new UserRole();

                        string[] dateInitInp = dob.Split('/');
                        string[] formatDateInitInp = new string[dateInitInp.Length];

                        formatDateInitInp[0] = dateInitInp[1];
                        formatDateInitInp[1] = dateInitInp[0];
                        formatDateInitInp[2] = dateInitInp[2];

                        string formattedDate = string.Join("-", formatDateInitInp);

                        DateTime dateOfBirthModel = new DateTime();

                        if (DateTime.TryParseExact(formattedDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthModel))
                        {
                            user = u;
                            user.Email = existingUser.Email;
                            user.EmailConfirmationToken = existingUser.EmailConfirmationToken;
                            user.EmailConfirmed = existingUser.EmailConfirmed;
                            user.BirthDate = dateOfBirthModel;
                            if (profilePic != null)
                            {
                                if (profilePic != null && profilePic.ContentLength > 0)
                                {
                                    string fileName = Path.GetFileName(profilePic.FileName);
                                    string uniqueFileName = GetUniqueFileName("~/Content/UsersProfileImages/", fileName);
                                    string filePath = Path.Combine(Server.MapPath("~/Content/UsersProfileImages/"), uniqueFileName);

                                    profilePic.SaveAs(filePath);

                                    user.ProfilePicturePath = "~/Content/UsersProfileImages/" + uniqueFileName;
                                }
                                else
                                {
                                    return Json(new { success = false, message = "An error has occured" });
                                }
                            }
                            else
                            {
                                user.ProfilePicturePath = existingUser.ProfilePicturePath;
                            }
                            userRole = _db.UserRole.Where(model => model.UserID == u.userID).FirstOrDefault();
                            userRole.RoleID = rolenameInt;
                            _userRepo.Update(user.userID, user);
                            try
                            {
                                _userRoleRepo.Update(userRole.UserRoleID, userRole);

                                return Json(new { success = true, message = "Profile updated successfully" });
                            }
                            catch (Exception)
                            {
                                _userRepo.Delete(user.userID);
                                return Json(new { success = false, message = "An error has occured" });
                                throw;
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Invalid date format" });
                        }

                    }
                    catch (Exception)
                    {
                        return View();
                        throw;
                    }
                }
                else
                {
                    return RedirectToAction("../Account/LogOut");
                }
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult ViewUserDetails(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userView = new UserViewModel();
                userView.user = _db.User.Where(model => model.userID == id).FirstOrDefault();
                userView.userRole = _db.UserRole.Where(model => model.UserID == id).FirstOrDefault();
                userView.userRoleView = _db.vw_RoleView.Where(model => model.RoleID == userView.userRole.RoleID).FirstOrDefault();
                userView.userPremium = _db.UserPremium.Where(model => model.UserID == userView.user.userID).FirstOrDefault();
                userView.userRecipe = _db.UserRecipe.Where(model => model.UserID == userView.user.userID).ToList();
                var userRecipe = _db.UserRecipe.Where(model => model.UserID == userView.user.userID).FirstOrDefault();


                if (userView.userPremium != null)
                {
                    userView.premiumType = _db.Premium.Where(model => model.PremiumID == userView.userPremium.PremiumID).FirstOrDefault();
                }
                else
                {
                    userView.premiumType = null;
                }

                if (userRecipe != null)
                {
                    userView.foodSale = _db.FoodSale.Where(model => model.UserRecipeID == userRecipe.UserRecipeID).ToList();

                } else
                {
                    userView.foodSale = null;
                }

                return View(userView);
            }
            return RedirectToAction("../Account/Login");
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    _userRepo.Delete(id);
                    return Json(new { success = true});
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                    throw;
                }
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        public ActionResult ApproveRecipe(int id)
        {
            if (Session["User"] != null)
            {
                var adminInCharge = Session["User"] as User;
                var uploader = _db.UserRecipe.Where(model => model.RecipeID == id).FirstOrDefault();
                var uploaderUserProfile = _db.User.Where(model => model.userID == uploader.UserID).FirstOrDefault();
                var recipe = _db.Recipe.Where(model => model.RecipeID == id).FirstOrDefault();

                recipe.IsApproved = true;
                recipe.ApprovedBy = adminInCharge.userID;
                recipe.DateApproved = DateTime.Now;

                try
                {
                    _recipeRepo.Update(id, recipe);
                    string email = ConfigurationManager.AppSettings["Email"];
                    string password = "cmobmopkqmvqdriu";
                    string noreplyEmail = "no-reply@moodbite.com";

                    //string confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = user.EmailConfirmationToken }, Request.Url.Scheme);

                    string subject = "Recipe Approval";

                    ////use this when deployed
                    //string body = $"Please confirm your email address by clicking <a href=\"{confirmationLink}\">here</a>";

                    string body = $"Your uploaded recipe {recipe.RecipeName} has been approved by the admins. Check it out!";

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(noreplyEmail);
                        message.To.Add(uploaderUserProfile.Email);
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
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                    throw;
                }
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
            
        }

        [HttpPost]
        public ActionResult RejectRecipe(int id)
        {
            if (Session["User"] != null)
            {
                var adminInCharge = Session["User"] as User;
                var recipe = _db.Recipe.Where(model => model.RecipeID == id).FirstOrDefault();
                var uploader = _db.UserRecipe.Where(model => model.RecipeID == id).FirstOrDefault();
                var uploaderUserProfile = _db.User.Where(model => model.userID == uploader.UserID).FirstOrDefault();

                recipe.IsApproved = false;
                recipe.ApprovedBy = adminInCharge.userID;
                recipe.DateApproved = DateTime.Today;

                try
                {
                    _recipeRepo.Update(id, recipe);

                    string email = ConfigurationManager.AppSettings["Email"];
                    string password = "cmobmopkqmvqdriu";
                    string noreplyEmail = "no-reply@moodbite.com";

                    //string confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = user.EmailConfirmationToken }, Request.Url.Scheme);

                    string subject = "Recipe Approval";

                    ////use this when deployed
                    //string body = $"Please confirm your email address by clicking <a href=\"{confirmationLink}\">here</a>";

                    string body = $"Your uploaded recipe {recipe.RecipeName} has been rejected by the admins!";

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(noreplyEmail);
                        message.To.Add(uploaderUserProfile.Email);
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
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                    throw;
                }
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult DeleteRecipe(int id)
        {
            if (Session["User"] != null)
            {
                var adminInCharge = Session["User"] as User;
                var userRecipe = _db.UserRecipe.Where(model => model.RecipeID == id).FirstOrDefault();
                var recipe = _db.Recipe.Where(model => model.RecipeID == id).FirstOrDefault();
                var uploader = _db.UserRecipe.Where(model => model.RecipeID == id).FirstOrDefault();
                var uploaderUserProfile = _db.User.Where(model => model.userID == uploader.UserID).FirstOrDefault();

                recipe.IsApproved = false;
                recipe.ApprovedBy = adminInCharge.userID;
                recipe.DateApproved = DateTime.Today;

                try
                {
                    _userRecipeRepo.Delete(userRecipe.UserRecipeID);
                    _recipeRepo.Delete(id);

                    string email = ConfigurationManager.AppSettings["Email"];
                    string password = "cmobmopkqmvqdriu";
                    string noreplyEmail = "no-reply@moodbite.com";

                    //string confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = user.EmailConfirmationToken }, Request.Url.Scheme);

                    string subject = "Recipe Approval";

                    ////use this when deployed
                    //string body = $"Please confirm your email address by clicking <a href=\"{confirmationLink}\">here</a>";

                    string body = $"Your uploaded recipe {recipe.RecipeName} has been rejected and deleted by an admin!";

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(noreplyEmail);
                        message.To.Add(uploaderUserProfile.Email);
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
                    return View("Index");
                }
                catch (Exception)
                {
                    return View("Index");
                    throw;
                }
            }
            else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult EditRecipe(int id)
        {
            if(User.Identity.IsAuthenticated)
            {
                var recipe = _recipeRepo.Get(id);
                var recipeIngredients = _db.RecipeIngredient.Where(m => m.RecipeID == id).ToList();
                var recipeImage = _db.RecipeImage.Where(m => m.RecipeID == recipe.RecipeID).FirstOrDefault();
                var userRecipe = _db.UserRecipe.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                var uploaderInfo = _db.User.Where(model => model.userID == userRecipe.UserID).FirstOrDefault();
                var foodCategory = _db.FoodCategory.ToList();

                Session["recipeIngredients"] = recipeIngredients;
                Session["recipeImage"] = recipeImage;
                Session["uploaderInfo"] = uploaderInfo;
                Session["FoodCategory"] = foodCategory;

                return View(recipe);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditRecipe(Recipe recipe, string foodcategory, string ingcount, string moodid, string[] ingredientName, int[] ingredientQty, string[] ingredientUnit)
        {
            if (Session["recipeIngredients"] != null && Session["recipeImage"] != null && Session["uploaderInfo"] != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var imageFile = Request.Files.Get("imageFile");
                    var existingRecipe = _db.Recipe.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                    recipe.IngredientsCount = Convert.ToInt32(ingcount);
                    recipe.MoodID = Convert.ToInt32(moodid);
                    recipe.IsApproved = existingRecipe.IsApproved;
                    recipe.DateApproved = existingRecipe.DateApproved;
                    recipe.ApprovedBy = existingRecipe.ApprovedBy;
                    recipe.DateUploaded = existingRecipe.DateUploaded;
                    recipe.FoodCategoryID = Convert.ToInt32(foodcategory);

                    try
                    {
                        _recipeRepo.Update(recipe.RecipeID, recipe);

                        var exisitngRecipeIngredient = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();


                        foreach (var ri in exisitngRecipeIngredient)
                        {
                            try
                            {
                                _recipeIngredientRepo.Delete(ri.RecipeIngredientID);
                            }
                            catch (Exception)
                            {
                                return View();
                                throw;
                            }
                        }


                        var recipeIngredient = new RecipeIngredient();
                        recipeIngredient.RecipeID = recipe.RecipeID;
                        for (int i = 0; i < Convert.ToInt32(ingcount); i++)
                        {
                            recipeIngredient.IngredientName = ingredientName[i];
                            recipeIngredient.Quantity = ingredientQty[i];
                            recipeIngredient.Unit = ingredientUnit[i];
                            _recipeIngredientRepo.Create(recipeIngredient);
                        }

                        if (imageFile != null)
                        {
                            var recipeImage = _db.RecipeImage.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();

                            string fileName = Path.GetFileName(imageFile.FileName);
                            string uniqueFileName = GetUniqueFileName("~/Content/RecipeImages/", fileName);
                            string filePath = Path.Combine(Server.MapPath("~/Content/RecipeImages/"), uniqueFileName);

                            imageFile.SaveAs(filePath);
                            recipeImage.ImageName = recipe.RecipeName + " cover";
                            recipeImage.ImagePath = "~/Content/RecipeImages/" + uniqueFileName;

                            try
                            {
                                _recipeImageRepo.Update(recipeImage.RecipeImageID, recipeImage);
                            }
                            catch (Exception)
                            {
                                return RedirectToAction("../Error/PageNotFound");
                                throw;
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Index");
                        throw;
                    }
                } else
                {
                    return RedirectToAction("../Account/LogOut");
                }
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult RecipeDetails(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var recipe = _recipeRepo.Get(id);
                var recipeIngredients = _db.RecipeIngredient.Where(m => m.RecipeID == id).ToList();
                var recipeImage = _db.RecipeImage.Where(m => m.RecipeID == recipe.RecipeID).FirstOrDefault();
                var userRecipe = _db.UserRecipe.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                var uploaderInfo = _db.User.Where(model => model.userID == userRecipe.UserID).FirstOrDefault();
                var foodCategory = _db.FoodCategory.ToList();
                var userPremium = _db.UserPremium.Where(model => model.UserID == uploaderInfo.userID).FirstOrDefault();

                Session["recipeIngredients"] = recipeIngredients;
                Session["recipeImage"] = recipeImage;
                Session["uploaderInfo"] = uploaderInfo;
                Session["foodCategory"] = foodCategory;
                //Session["userPremium"] = userPremium;

                return View(recipe);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ManageSubscriptions()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_db.vw_ManageSubscriptions.ToList());
            }
            return RedirectToAction("./Account/Login");
        }
    }
}