using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Rotativa;
using MoodBite.Models.RecipeViewModel;
using System.Net.Http.Headers;
using System.Collections.Generic;
using MoodBite.Models.FoodSaleModel;
using System.Globalization;
using System.Reflection;
using MoodBite.Contract;
using System.Text.RegularExpressions;

namespace MoodBite.Controllers
{
    [HandleError]
    [Authorize(Roles = "User, Admin")]
    [OutputCache(NoStore = true, Duration = 0)]
    public class UsersPageController : BaseController
    {
        public string localChosenMood;
        //modal pop up which prompts the user to select a mood
        
        public ActionResult ChooseMood()
        {
            if(User.Identity.IsAuthenticated)
            {
                var moodList = _db.Mood.ToList();
                return View(moodList);
            }
            return RedirectToAction("../Account/LogOut");
        }

        //handles the submission for mood selection
        [HttpPost]
        public ActionResult ChooseMood(string chosenMood)
        {
            if (User.Identity.IsAuthenticated)
            {
                Session["ChosenMood"] = chosenMood;
                this.localChosenMood = chosenMood;
                //return RedirectToAction("UsersHome");
                return RedirectToAction("UsersHome");
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        //after selecting mood user will be redirected here, where recipes will be generated based on mood inputted
        public ActionResult UsersHome()
        {
            if (User.Identity.IsAuthenticated && Session["ChosenMood"] != null)
            {
                var chosenMood = Session["ChosenMood"].ToString();
                var recipedetail = RecipeDetail(chosenMood);
                Session["SearchInput"] = string.Empty;
                if (TempData["recipedetail"] == null)
                {
                    return View(recipedetail);
                } else
                {
                    var tempRecipeDetail = TempData["recipedetail"] as RecipeDetailViewModel;
                    return View(tempRecipeDetail);
                }
            }
            else if (User.Identity.IsAuthenticated && Session["ChosenMood"] == null)
            {
                return RedirectToAction("ChooseMood");
            }
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        public ActionResult UsersHome(string search, string[] allergyInp, string[] intoleranceInp, string[] foodCategoryInp)
        {
            if (User.Identity.IsAuthenticated && Session["ChosenMood"] != null)
            {
                if(string.IsNullOrWhiteSpace(search))
                {
                    var chosenMood = Session["ChosenMood"] as String;
                    var user = Session["User"] as User;
                    var recipedetail = RecipeDetail(chosenMood);
                    TempData["recipedetail"] = recipedetail;
                    Session["SearchInput"] = search;
                    return Json(new { success = true});
                } else
                {
                    var chosenMood = Session["ChosenMood"] as String;
                    var user = Session["User"] as User;
                    var recipedetail = RecipeDetail(chosenMood, search, allergyInp, intoleranceInp, foodCategoryInp);
                    TempData["recipedetail"] = recipedetail;
                    Session["SearchInput"] = search;
                    return Json(new { success = true });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Session timeout."});
            }
        }

        //view full details when read more button is clicked from the recipe card/item
        public ActionResult RecipeReadMore(int id)
        {
            if (!string.IsNullOrEmpty(Session["returnurl"] as string))
            {
                var returnurl = Session["returnurl"] as string;
                returnurl = returnurl.Replace("/UsersPage/RecipeReadMore/", "");
                var recipeDetail = ReadMore(id);
                var user = Session["User"] as User;
                if (recipeDetail == null)
                {
                    return RedirectToAction("../Error/PageNotFound");
                }
                return View(recipeDetail);
            }
            if (Session["ChosenMood"] == null || Session["User"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            }
            else if(User.Identity.IsAuthenticated && Session["ChosenMood"] != null && Session["User"] != null)
            {
                var recipeDetail = ReadMore(id);
                var user = Session["User"] as User;
                if (recipeDetail == null)
                {
                    return RedirectToAction("../Error/PageNotFound");
                }
                return View(recipeDetail);
            }
            else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        [HttpPost]
        public ActionResult RecipeReadMore(int recipeID, int userID)
        {
            return Json(new { success = true, message = "Product has been added to your shopping cart." });
        }

        public ActionResult UploadRecipe()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _db.User.Where(m => m.Username == User.Identity.Name).FirstOrDefault();
                var model = new Recipe();
                var userPremium = _db.UserPremium.Where(m => m.UserID == user.userID).FirstOrDefault();
                var foodCategory = _db.FoodCategory.ToList();
                Session["UserPremium"] = userPremium;
                Session["FoodCategory"] = foodCategory;
                return View(model);
            }
            else
            {
                return RedirectToAction("../Account/LogOuts");
            }
           
        }

        [HttpPost]
        public ActionResult UploadRecipe(Recipe recipe, string foodcategory, string ingcount, string moodid, string[] ingredientName, double[] ingredientQty, string[] ingredientUnit, double? price, string shippedfrom, string stock, string forSale)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".svg" };
            var imageFile = Request.Files.Get("imageFile");
            string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            TimeSpan preparationTime;
            TimeSpan cookingDuration;

            if (string.IsNullOrEmpty(recipe.RecipeName))
            {
                return Json(new { success = false, msg = "Please fill in the recipe name."});
            }

            if (string.IsNullOrEmpty(recipe.RecipeDescription))
            {
                return Json(new { success = false, msg = "Please fill in the recipe description." });
            }

            if (string.IsNullOrEmpty(Convert.ToString(recipe.PreparationTime)))
            {
                return Json(new { success = false, msg = "Please fill in the preparation time." });
            }

            string timePattern = @"^(?<hours>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2})$";
            Match preparationTimeMatch = Regex.Match(recipe.PreparationTime.ToString(), timePattern);
            if (!preparationTimeMatch.Success)
            {
                return Json(new { success = false, msg = "Invalid preparation time format. Expected format: hh:mm:ss" });
            }

            int preparationHours = int.Parse(preparationTimeMatch.Groups["hours"].Value);
            int preparationMinutes = int.Parse(preparationTimeMatch.Groups["minutes"].Value);
            int preparationSeconds = int.Parse(preparationTimeMatch.Groups["seconds"].Value);

            if (preparationHours < 0 || preparationHours > 23 || preparationMinutes < 0 || preparationMinutes > 59 || preparationSeconds < 0 || preparationSeconds > 59)
            {
                return Json(new { success = false, msg = "Invalid preparation time. Please enter a valid time." });
            }

            preparationTime = new TimeSpan(preparationHours, preparationMinutes, preparationSeconds);
            if (preparationTime.TotalMinutes < 1)
            {
                return Json(new { success = false, msg = "Preparation time should be at least 1 minute." });
            }

            if (string.IsNullOrEmpty(Convert.ToString(recipe.CookingDuration)))
            {
                return Json(new { success = false, msg = "Please fill in the cooking duration." });
            }

            Match cookingDurationMatch = Regex.Match(recipe.CookingDuration.ToString(), timePattern);
            if (!cookingDurationMatch.Success)
            {
                return Json(new { success = false, msg = "Invalid cooking duration time format. Expected format: hh:mm:ss" });
            }

            int cookingHours = int.Parse(cookingDurationMatch.Groups["hours"].Value);
            int cookingMinutes = int.Parse(cookingDurationMatch.Groups["minutes"].Value);
            int cookingSeconds = int.Parse(cookingDurationMatch.Groups["seconds"].Value);

            if (cookingHours < 0 || cookingHours > 23 || cookingMinutes < 0 || cookingMinutes > 59 || cookingSeconds < 0 || cookingSeconds > 59)
            {
                return Json(new { success = false, msg = "Invalid cooking duration. Please enter a valid time." });
            }

            cookingDuration = new TimeSpan(cookingHours, cookingMinutes, cookingSeconds);
            if (cookingDuration.TotalMinutes < 1)
            {
                return Json(new { success = false, msg = "Cooking duration should be at least 1 minute." });
            }

            if (string.IsNullOrEmpty(ingcount) || Convert.ToInt32(ingcount) <= 0)
            {
                return Json(new { success = false, msg = "Please include at least 1 ingredient." });
            }

            if (string.IsNullOrEmpty(recipe.CookingInstruction))
            {
                return Json(new { success = false, msg = "Please fill in cooking instruction." });
            }

            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return Json(new { success = false, msg = "Please add an image to visualize recipe." });
            }

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Json(new { success = false, msg = "Invalid image file format. Please choose a JPEG, PNG, BMP, WEBP, or SVG file." });
            }

            if (forSale.ToLower() == "yes")
            {
                if (price <= 0 || price == null)
                {
                    return Json(new { success = false, msg = "Price should be greater than zero." });
                }
                if (Convert.ToDouble(stock) <= 1 || string.IsNullOrEmpty(stock))
                {
                    return Json(new { success = false, msg = "Should have at least 2 stocks." });
                }
                if (string.IsNullOrEmpty(shippedfrom))
                {
                    return Json(new { success = false, msg = "Please fill in the address the product is shipped from." });
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
                var userRecipe = new UserRecipe();
                var foodSale = new FoodSale();
                var recipeIngredient = new RecipeIngredient();
                var recipeImage = new RecipeImage();
                var vw_StarterUser = _db.vw_StarterUser.Where(model => model.UserID == user.userID && model.PremiumID == 1).ToList();
                recipe.FoodCategoryID = Convert.ToInt32(foodcategory);
                recipe.IngredientsCount = Convert.ToInt32(ingcount);
                recipe.MoodID = Convert.ToInt32(moodid);
                recipe.DateUploaded = DateTime.Now;
                recipe.IsApproved = false;
                if (vw_StarterUser.Count() < 2)
                {
                    if (forSale.ToLower() == "no")
                    {
                        if (_recipeRepo.Create(recipe) == ErrorCode.Success)
                        {
                            recipeImage.RecipeID = recipe.RecipeID;
                            recipeImage.ImageName = recipe.RecipeName + " cover";
                            string fileName = Path.GetFileName(imageFile.FileName);
                            string uniqueFileName = GetUniqueFileName("~/Content/RecipeImages/", fileName);
                            string filePath = Path.Combine(Server.MapPath("~/Content/RecipeImages/"), uniqueFileName);
                            imageFile.SaveAs(filePath);
                            recipeImage.ImagePath = "~/Content/RecipeImages/" + uniqueFileName;
                            if (_recipeImageRepo.Create(recipeImage) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }
                            for (int i = 1; i < recipe.IngredientsCount + 1; i++)
                            {
                                recipeIngredient.RecipeID = recipe.RecipeID;
                                int ingredientNameInt;
                                if (int.TryParse(ingredientName[i], out ingredientNameInt) || string.IsNullOrEmpty(ingredientName[i]))
                                {
                                    var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                    if (forDeleteIngredients.Count <= 0)
                                    {
                                        _recipeRepo.Delete(recipe.RecipeID);
                                        return Json(new { success = false, msg = "Invalid ingredient name." });
                                    }
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid ingredient name." });
                                }
                                recipeIngredient.IngredientName = ingredientName[i];
                                recipeIngredient.Unit = ingredientUnit[i];
                                if (ingredientQty[i] <= 0 || string.IsNullOrEmpty(Convert.ToString(ingredientQty[i])))
                                {
                                    var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                    if (forDeleteIngredients.Count <= 0)
                                    {
                                        _recipeRepo.Delete(recipe.RecipeID);
                                        return Json(new { success = false, msg = "Invalid qty input." });
                                    }
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid qty input." });
                                }
                                recipeIngredient.Quantity = ingredientQty[i];
                                if (_recipeIngredientRepo.Create(recipeIngredient) == ErrorCode.Error)
                                {
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                                }
                            }
                            userRecipe.RecipeID = recipe.RecipeID;
                            userRecipe.UserID = user.userID;
                            if (_userRecipeRepo.Create(userRecipe) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count > 0)
                                {
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                }
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }
                            return Json(new { success = true, msg = "Your recipe has been successfully uploaded. It is currently awaiting approval from the administrators before it can be officially published." });
                        }
                        else
                        {
                            return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                        }
                    }

                    if (forSale.ToLower() == "yes")
                    {
                        if (_recipeRepo.Create(recipe) == ErrorCode.Success)
                        {
                            recipeImage.RecipeID = recipe.RecipeID;
                            recipeImage.ImageName = recipe.RecipeName + " cover";
                            string fileName = Path.GetFileName(imageFile.FileName);
                            string uniqueFileName = GetUniqueFileName("~/Content/RecipeImages/", fileName);
                            string filePath = Path.Combine(Server.MapPath("~/Content/RecipeImages/"), uniqueFileName);
                            imageFile.SaveAs(filePath);

                            recipeImage.ImagePath = "~/Content/RecipeImages/" + uniqueFileName;
                            if (_recipeImageRepo.Create(recipeImage) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }
                            for (int i = 1; i < recipe.IngredientsCount + 1; i++)
                            {
                                recipeIngredient.RecipeID = recipe.RecipeID;
                                int ingredientNameInt;
                                if (int.TryParse(ingredientName[i], out ingredientNameInt) || string.IsNullOrEmpty(ingredientName[i]))
                                {
                                    var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                    if (forDeleteIngredients.Count <= 0)
                                    {
                                        _recipeRepo.Delete(recipe.RecipeID);
                                        return Json(new { success = false, msg = "Invalid ingredient name." });
                                    }
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid ingredient name." });
                                }
                                recipeIngredient.IngredientName = ingredientName[i];
                                recipeIngredient.Unit = ingredientUnit[i];
                                if (ingredientQty[i] <= 0 || string.IsNullOrEmpty(Convert.ToString(ingredientQty[i])))
                                {
                                    var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                    if (forDeleteIngredients.Count <= 0)
                                    {
                                        _recipeRepo.Delete(recipe.RecipeID);
                                        return Json(new { success = false, msg = "Invalid qty input." });
                                    }
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid qty input." });
                                }
                                recipeIngredient.Quantity = ingredientQty[i];
                                if (_recipeIngredientRepo.Create(recipeIngredient) == ErrorCode.Error)
                                {
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                                }
                            }
                            userRecipe.RecipeID = recipe.RecipeID;
                            userRecipe.UserID = user.userID;
                            if (_userRecipeRepo.Create(userRecipe) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count > 0)
                                {
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                }
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }

                            foodSale.UserRecipeID = userRecipe.UserRecipeID;
                            foodSale.Price = Convert.ToDecimal(price);
                            foodSale.Address = shippedfrom;
                            foodSale.Available = true;
                            foodSale.Stocks = Convert.ToInt32(stock);
                            if (_foodSaleRepo.Create(foodSale) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count > 0)
                                {
                                    foreach (var del in forDeleteIngredients)
                                    {
                                        _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                    }
                                }
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }

                            return Json(new { success = true, msg = "Your recipe has been successfully uploaded. It is currently awaiting approval from the administrators before it can be officially published." });
                        }
                        else
                        {
                            return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                        }
                    }
                } else
                {
                    return Json(new { success = false, msg = "Your plan is only limited to 2 sellable uploads" });
                }
            }
            return Json(new { success = false, msg = "Session timeout." });
        }

        public ActionResult ViewForDownloadRecipe(int id)
        {
            if (Session["ChosenMood"] == null || Session["User"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            }
            else if (User.Identity.IsAuthenticated && Session["ChosenMood"] != null && Session["User"] != null)
            {
                var recipeDetail = ReadMore(id);
                var user = Session["User"] as User;
                if (recipeDetail == null)
                {
                    return RedirectToAction("../Error/PageNotFound");
                }
                return View(recipeDetail);
            }
            else
            {
                return RedirectToAction("../Home/Index");
            }
        }


        public ActionResult DownloadRecipe(int id)
        {
            if (Session["ChosenMood"] == null || Session["User"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            }
            else if (User.Identity.IsAuthenticated && Session["ChosenMood"] != null && Session["User"] != null)
            {
                var recipeDetail = ReadMore(id);
                var user = Session["User"] as User;
                if (recipeDetail == null)
                {
                    return RedirectToAction("../Error/PageNotFound");
                }

                //string currentUrl = Request.Url.AbsoluteUri;

                return new ViewAsPdf("ViewForDownloadRecipe", recipeDetail) { FileName = recipeDetail.recipeReadMore.RecipeName + " recipe.pdf" };
            }
            else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        [HttpPost]
        public ActionResult SubmitRating(int rating, int recipeID)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            }
            else if ((User.Identity.IsAuthenticated && Session["User"] != null))
            {
                var user = Session["User"] as User;
                RecipeRating rr = new RecipeRating();

                var checkIfAlreadyRatedCurrentRecipe = _db.RecipeRating.Where(model => model.UserID == user.userID && model.RecipeID == recipeID).ToList();


                //if this variable is less than 0 it means that this user has no record of rating of this current recipe
                if(checkIfAlreadyRatedCurrentRecipe.Count() <= 0)
                {
                    //recipeid that will be rated
                    rr.RecipeID = recipeID;

                    //userid of the rater
                    rr.UserID = user.userID;

                    //rate submitted from readmore view via ajax
                    rr.Rate = rating;
                    try
                    {
                        _recipeRating.Create(rr);
                        return Json(new { success = true, message = $"Rating submitted successfully! ({rating})" });
                    }
                    catch (Exception)
                    {
                        return Json(new { success = false, message = "Failed to submit rating!" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "You have already rated this recipe!" });
                }
            }
            else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        public ActionResult MyCart()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            } else
            {
                var user = Session["User"] as User;
                var userPremium = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();

                if (userPremium != null)
                {
                    var myCartItem = _db.vw_CartView.Where(model => model.userID == user.userID).ToList();
                    return View(myCartItem);
                } else
                {
                    return RedirectToAction("UsersHome");
                }
                
            }
        }

        [HttpPost]
        public ActionResult AddToCart(int recipeID)
        {
            if(Session["User"] == null)
            {
                return Json(new { success = false, message = "An error has occured." });
            } else
            {
                var user = Session["User"] as User;
                var recipe = _db.Recipe.Where(model => model.RecipeID == recipeID).FirstOrDefault();
                var productDetails = _db.vw_FoodSaleView.Where(model => model.RecipeID == recipeID).FirstOrDefault();

                Cart cart = new Cart();
                cart.UserID = user.userID;
                cart.RecipeID = recipe.RecipeID;
                cart.FoodSaleID = productDetails.FoodSaleID;
                cart.Qty = 1;
                try
                {
                    var itemExist = _db.Cart.Where(model => model.RecipeID == recipeID && model.UserID == user.userID).FirstOrDefault();

                    if(itemExist != null)
                    {
                        itemExist.Qty++;
                        _cartRepo.Update(itemExist.CartID, itemExist);
                        return Json(new { success = true, message = "Ordered product qty has been updated to your shopping cart." });
                    }
                    else
                    {
                        _cartRepo.Create(cart);
                        return Json(new { success = true, message = "Product has been added to your shopping cart." });
                    }

                } catch (Exception)
                {
                    return Json(new { success = false, message = "An error occurred while adding the product to the cart." });
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveItem(int itemID)
        {
            if (Session["User"] != null)
            {
                try
                {
                    _cartRepo.Delete(itemID);
                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int cartID, int newQty)
        {
            if(Session["User"] != null)
            {
                var user = Session["User"] as User;

                var cartModel = _db.Cart.Where(model => model.CartID == cartID).FirstOrDefault();
                Cart cart = new Cart();
                cart = cartModel;

                cart.Qty = newQty;

                try
                {
                    if(cart.Qty == 0)
                    {
                        _cartRepo.Delete(cartID);
                        return Json(new { success = true, zeroUnit = true });
                    }
                    else
                    {
                        _cartRepo.Update(cartID, cart);
                        return Json(new { success = true, zeroUnit = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = false});
                }
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        public ActionResult GetTotalPrice(int cartId)
        {
            if (Session["User"] != null)
            {
                var user = Session["User"] as User;

                decimal totalPrice = Convert.ToDecimal(_db.vw_CartView.Where(model => model.CartID == cartId).Select(model => model.Total_Price).FirstOrDefault());

                decimal allItemsTotalPrice = Convert.ToDecimal(_db.vw_CartView.Where(model => model.CartID == cartId).Sum(model => model.Total_Price));

                return Json(new { TotalPrice = totalPrice, AllItemsTotalPrice = allItemsTotalPrice});
            }
            else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        //id parameter is userId
        public ActionResult CheckOutPage(int id)
        {
            var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
            var cartItem = _db.vw_CheckOutView.Where(model => model.CartID == id && model.userID == user.userID).FirstOrDefault();

            if (cartItem != null)
            {
                if (Session["User"] != null)
                {
                    return View(cartItem);
                }
                else
                {
                    return RedirectToAction("../Error/PageNotFound");
                }
            } else
            {
                return RedirectToAction("../Error/PageNotFound");
            }
        }

        [HttpPost]
        //id = userid
        public ActionResult PlaceOrder(int id, string address, string paymentMethod, decimal totalAmount, int deliveryFee, int estArrivalTime)
            {
            if (Session["User"] != null)
            {
                if ((id == 0) || (String.IsNullOrEmpty(address)) || String.IsNullOrEmpty(paymentMethod) || totalAmount == 0)
                {
                    return Json(new { success = false });
                } else
                {
                    var cart = _db.vw_CheckOutView.Where(model => model.userID == id).ToList();
                    var user = Session["User"] as User;

                    OrderMaster om = new OrderMaster();
                    om.CustomerID = cart.ElementAt(0).userID;
                    om.DateOrdered = DateTime.Today;
                    om.CustomerAddress = address;
                    om.IsPaid = false;
                    om.EstimatedArrivalTime = estArrivalTime;
                    om.DeliveryFee = deliveryFee;
                    _orderMasterRepo.Create(om);

                    var omId = om.PO_ID;

                    OrderDetail od = new OrderDetail();

                    foreach (var item in cart)
                    {
                        var foodSale = _db.FoodSale.Where(model => model.FoodSaleID == item.FoodSaleID).FirstOrDefault();
                        od.PO_ID = omId;
                        od.FoodForSaleID = item.FoodSaleID;
                        od.Quantity = item.Qty;
                        od.UnitPrice = item.Price;
                        od.TotalPrice = item.Total_Price + deliveryFee;

                        _orderDetailRepo.Create(od);

                        foodSale.Stocks = foodSale.Stocks - item.Qty;
                        if (foodSale.Stocks <= 0)
                        {
                            foodSale.Available = false;
                            foodSale.Stocks = 0;
                        }
                        _foodSaleRepo.Update(foodSale.FoodSaleID, foodSale);
                    }

                    var totalAmountToBePaid = _db.vw_ForOrderPaymentInsertionView.Where(model => model.CustomerID == id).Sum(model => model.TotalPrice);
                    OrderPayment op = new OrderPayment();
                    op.AmountToBePaid = totalAmount;
                    op.ModeOfPayment = paymentMethod;

                    switch (paymentMethod.ToLower())
                    {
                        case "visa":
                        case "gcash":
                        case "paymaya":
                            op.AmountPaid = 99999;
                            op.DatePaid = DateTime.Today;

                            //update ispaid status if paid
                            var myOrder = _db.OrderMaster.Where(model => model.CustomerID == id).ToList();
                            if (myOrder != null || myOrder.Count > 0)
                            {
                                foreach (var item in myOrder)
                                {
                                    item.IsPaid = true;
                                    _orderMasterRepo.Update(item.PO_ID, item);
                                }
                            }
                            break;
                        case "cod":
                            op.AmountPaid = 0;
                            break;
                        default:
                            return RedirectToAction("CheckOutPage");
                    }

                    var removeFromCart = _db.Cart.Where(model => model.UserID == id).ToList();

                    foreach (var item in removeFromCart)
                    {
                        _cartRepo.Delete(item.CartID);
                    }

                    return Json(new { success = true });
                }
            }
            else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        public ActionResult AddToFavorites(int recipeID)
        {

            if (Session["User"] != null)
            {
                var user = Session["User"] as User;
                UsersFavoriteRecipes fave = new UsersFavoriteRecipes();
                fave.UserID = user.userID;
                fave.RecipeID = recipeID;

                var existingFavorites = _db.UsersFavoriteRecipes.Where(model => model.UserID == user.userID && model.RecipeID == recipeID).FirstOrDefault();

                try
                {
                    if (existingFavorites == null)
                    {
                        _userFaveRecipe.Create(fave);
                        return Json(new { success = true, message = "Added to favorites!", add = true });
                    }
                    else
                    {
                        _userFaveRecipe.Delete(existingFavorites.UsersFaveRecipeID);
                        return Json(new { success = true, message = "Removed from favorites!", add = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "An error has occured."});
                    throw;
                }

            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult MyUploads()
        {
            if (User.Identity.IsAuthenticated)
            {
                FooSaleViewModel foodSaleViewModel = new FooSaleViewModel();
                var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
                var userRecipeView = _db.vw_UserRecipeView.Where(model => model.userID == user.userID).ToList();
                var foodSale = new List<vw_UserRecipeView>();

                foreach (var item in userRecipeView)
                {
                    if (item.FoodSaleID != null)
                    {
                        foodSale.Add(item);
                    }
                }

                foodSaleViewModel.userRecipe = userRecipeView;
                foodSaleViewModel.foodSale = foodSale;
                Session["foodSale"] = foodSale;
                return View(foodSaleViewModel);
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult EditRecipe(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Session["foodSale"] != null)
                {
                    var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
                    var recipe = _recipeRepo.Get(id);
                    var recipeIngredients = _db.RecipeIngredient.Where(m => m.RecipeID == id).ToList();
                    var recipeImage = _db.RecipeImage.Where(m => m.RecipeID == recipe.RecipeID).FirstOrDefault();
                    var userRecipe = _db.UserRecipe.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                    var uploaderInfo = _db.User.Where(model => model.userID == userRecipe.UserID).FirstOrDefault();
                    var foodCategory = _db.FoodCategory.ToList();

                    if (user.userID == userRecipe.UserID)
                    {
                        Session["recipeIngredients"] = recipeIngredients;
                        Session["recipeImage"] = recipeImage;
                        Session["uploaderInfo"] = uploaderInfo;
                        Session["FoodCategory"] = foodCategory;

                        var foodSale = Session["foodSale"] as List<vw_UserRecipeView>;

                        vw_UserRecipeView recipeFoodSle = new vw_UserRecipeView();

                        var editFoodSale = new vw_UserRecipeView();

                        foreach (var item in foodSale)
                        {
                            if (item.RecipeID == id)
                            {
                                editFoodSale = item;
                                break;
                            }
                            else
                            {
                                editFoodSale = null;
                            }
                        }

                        Session["editFoodSale"] = editFoodSale;

                        return View(recipe);
                    } else
                    {
                        return RedirectToAction("../Error/PageNotFound");
                    }
                } else
                {
                    return RedirectToAction("../Account/LogOut");
                }
            }
            return RedirectToAction("../Account/LogOut");
        }

        [HttpPost]
        public ActionResult EditRecipe(Recipe recipe, string foodcategory, string ingcount, string moodid, string[] ingredientName, double[] ingredientQty, string[] ingredientUnit, double? price, string shippedfrom, string stock)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".svg" };
            var imageFile = Request.Files.Get("imageFile");
            string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            TimeSpan preparationTime;
            TimeSpan cookingDuration;

            if (string.IsNullOrEmpty(recipe.RecipeName))
            {
                return Json(new { success = false, msg = "Please fill in the recipe name." });
            }

            if (string.IsNullOrEmpty(recipe.RecipeDescription))
            {
                return Json(new { success = false, msg = "Please fill in the recipe description." });
            }

            if (string.IsNullOrEmpty(Convert.ToString(recipe.PreparationTime)))
            {
                return Json(new { success = false, msg = "Please fill in the preparation time." });
            }

            string timePattern = @"^(?<hours>\d{2}):(?<minutes>\d{2}):(?<seconds>\d{2})$";
            Match preparationTimeMatch = Regex.Match(recipe.PreparationTime.ToString(), timePattern);
            if (!preparationTimeMatch.Success)
            {
                return Json(new { success = false, msg = "Invalid preparation time format. Expected format: hh:mm:ss" });
            }

            int preparationHours = int.Parse(preparationTimeMatch.Groups["hours"].Value);
            int preparationMinutes = int.Parse(preparationTimeMatch.Groups["minutes"].Value);
            int preparationSeconds = int.Parse(preparationTimeMatch.Groups["seconds"].Value);

            if (preparationHours < 0 || preparationHours > 23 || preparationMinutes < 0 || preparationMinutes > 59 || preparationSeconds < 0 || preparationSeconds > 59)
            {
                return Json(new { success = false, msg = "Invalid preparation time. Please enter a valid time." });
            }

            preparationTime = new TimeSpan(preparationHours, preparationMinutes, preparationSeconds);
            if (preparationTime.TotalMinutes < 1)
            {
                return Json(new { success = false, msg = "Preparation time should be at least 1 minute." });
            }

            if (string.IsNullOrEmpty(Convert.ToString(recipe.CookingDuration)))
            {
                return Json(new { success = false, msg = "Please fill in the cooking duration." });
            }

            Match cookingDurationMatch = Regex.Match(recipe.CookingDuration.ToString(), timePattern);
            if (!cookingDurationMatch.Success)
            {
                return Json(new { success = false, msg = "Invalid cooking duration time format. Expected format: hh:mm:ss" });
            }

            int cookingHours = int.Parse(cookingDurationMatch.Groups["hours"].Value);
            int cookingMinutes = int.Parse(cookingDurationMatch.Groups["minutes"].Value);
            int cookingSeconds = int.Parse(cookingDurationMatch.Groups["seconds"].Value);

            if (cookingHours < 0 || cookingHours > 23 || cookingMinutes < 0 || cookingMinutes > 59 || cookingSeconds < 0 || cookingSeconds > 59)
            {
                return Json(new { success = false, msg = "Invalid cooking duration. Please enter a valid time." });
            }

            cookingDuration = new TimeSpan(cookingHours, cookingMinutes, cookingSeconds);
            if (cookingDuration.TotalMinutes < 1)
            {
                return Json(new { success = false, msg = "Cooking duration should be at least 1 minute." });
            }

            if (string.IsNullOrEmpty(ingcount) || Convert.ToInt32(ingcount) <= 0)
            {
                return Json(new { success = false, msg = "Please include at least 1 ingredient." });
            }

            if (string.IsNullOrEmpty(recipe.CookingInstruction))
            {
                return Json(new { success = false, msg = "Please fill in cooking instruction." });
            }

            //if (imageFile == null || imageFile.ContentLength <= 0)
            //{
            //    return Json(new { success = false, msg = "Please add an image to visualize recipe." });
            //}

            if (!string.IsNullOrEmpty(stock))
            {
                if (price <= 0 || price == null)
                {
                    return Json(new { success = false, msg = "Price should be greater than zero." });
                }
                if (string.IsNullOrEmpty(stock))
                {
                    return Json(new { success = false, msg = "Please fill in the stock" });
                }
                if (string.IsNullOrEmpty(shippedfrom))
                {
                    return Json(new { success = false, msg = "Please fill in the address the product is shipped from." });
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
                var userRecipe = new UserRecipe();
                var foodSale = new FoodSale();
                var recipeIngredient = new RecipeIngredient();
                var recipeImage = new RecipeImage();
                recipe.FoodCategoryID = Convert.ToInt32(foodcategory);
                recipe.IngredientsCount = Convert.ToInt32(ingcount);
                recipe.MoodID = Convert.ToInt32(moodid);

                if (string.IsNullOrEmpty(stock))
                {
                    if (_recipeRepo.Update(recipe.RecipeID, recipe) == ErrorCode.Success)
                    {
                        var sameRecipeImage = _db.RecipeImage.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                        var sameRecipeIngredient = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                        if (imageFile == null || imageFile.ContentLength <= 0)
                        {
                            recipeImage = sameRecipeImage;
                        } else
                        {
                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                return Json(new { success = false, msg = "Invalid image file format. Please choose a JPEG, PNG, BMP, WEBP, or SVG file." });
                            }
                            recipeImage.RecipeImageID = sameRecipeImage.RecipeImageID;
                            recipeImage.RecipeID = recipe.RecipeID;
                            recipeImage.ImageName = recipe.RecipeName + " cover";
                            string fileName = Path.GetFileName(imageFile.FileName);
                            string uniqueFileName = GetUniqueFileName("~/Content/RecipeImages/", fileName);
                            string filePath = Path.Combine(Server.MapPath("~/Content/RecipeImages/"), uniqueFileName);
                            imageFile.SaveAs(filePath);
                            recipeImage.ImagePath = "~/Content/RecipeImages/" + uniqueFileName;
                        }
                        if (_recipeImageRepo.Update(recipeImage.RecipeImageID, recipeImage) == ErrorCode.Error)
                        {
                            _recipeRepo.Delete(recipe.RecipeID);
                            return Json(new { success = false, msg = "An error has occured when updating your recipe, please try again." });
                        }
                        foreach (var samerecIngredient in sameRecipeIngredient)
                        {
                            _recipeIngredientRepo.Delete(samerecIngredient.RecipeIngredientID);
                        }
                        for (int i = 0; i < recipe.IngredientsCount; i++)
                        {
                            recipeIngredient.RecipeID = recipe.RecipeID;
                            int ingredientNameInt;
                            if (int.TryParse(ingredientName[i], out ingredientNameInt) || string.IsNullOrEmpty(ingredientName[i]))
                            {
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count <= 0)
                                {
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid ingredient name." });
                                }
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "Invalid ingredient name." });
                            }
                            recipeIngredient.IngredientName = ingredientName[i];
                            recipeIngredient.Unit = ingredientUnit[i];
                            if (ingredientQty[i] <= 0 || string.IsNullOrEmpty(Convert.ToString(ingredientQty[i])))
                            {
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count <= 0)
                                {
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid qty input." });
                                }
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "Invalid qty input." });
                            }
                            recipeIngredient.Quantity = ingredientQty[i];
                            if (_recipeIngredientRepo.Create(recipeIngredient) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }
                        }
                        var sameUserRecipe = _db.UserRecipe.Where(model => model.RecipeID == recipe.RecipeID && model.UserID == user.userID).FirstOrDefault();
                        userRecipe.UserRecipeID = sameUserRecipe.UserRecipeID;
                        userRecipe.RecipeID = recipe.RecipeID;
                        userRecipe.UserID = user.userID;
                        if (_userRecipeRepo.Update(userRecipe.UserRecipeID, userRecipe) == ErrorCode.Error)
                        {
                            _recipeRepo.Delete(recipe.RecipeID);
                            var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                            if (forDeleteIngredients.Count > 0)
                            {
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                            }
                            return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                        } 
                        return Json(new { success = true, msg = "Recipe updated successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                    }
                }

                if (!string.IsNullOrEmpty(stock))
                {
                    if (_recipeRepo.Update(recipe.RecipeID, recipe) == ErrorCode.Success)
                    {
                        var sameRecipeImage = _db.RecipeImage.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                        var sameRecipeIngredient = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                        if (imageFile == null || imageFile.ContentLength <= 0)
                        {
                            recipeImage = sameRecipeImage;
                        }
                        else
                        {
                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                return Json(new { success = false, msg = "Invalid image file format. Please choose a JPEG, PNG, BMP, WEBP, or SVG file." });
                            }
                            recipeImage.RecipeImageID = sameRecipeImage.RecipeImageID;
                            recipeImage.RecipeID = recipe.RecipeID;
                            recipeImage.ImageName = recipe.RecipeName + " cover";
                            string fileName = Path.GetFileName(imageFile.FileName);
                            string uniqueFileName = GetUniqueFileName("~/Content/RecipeImages/", fileName);
                            string filePath = Path.Combine(Server.MapPath("~/Content/RecipeImages/"), uniqueFileName);
                            imageFile.SaveAs(filePath);
                            recipeImage.ImagePath = "~/Content/RecipeImages/" + uniqueFileName;
                        }
                        if (_recipeImageRepo.Update(recipeImage.RecipeImageID, recipeImage) == ErrorCode.Error)
                        {
                            _recipeRepo.Delete(recipe.RecipeID);
                            return Json(new { success = false, msg = "An error has occured when updating your recipe, please try again." });
                        }
                        foreach (var samerecIngredient in sameRecipeIngredient)
                        {
                            _recipeIngredientRepo.Delete(samerecIngredient.RecipeIngredientID);
                        }
                        recipe.IngredientsCount = 0;
                        _recipeRepo.Update(recipe.RecipeID, recipe);
                        for (int i = 0; i < recipe.IngredientsCount; i++)
                        {
                            recipeIngredient.RecipeID = recipe.RecipeID;
                            int ingredientNameInt;
                            if (int.TryParse(ingredientName[i], out ingredientNameInt) || string.IsNullOrEmpty(ingredientName[i]))
                            {
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count <= 0)
                                {
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid ingredient name." });
                                }
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "Invalid ingredient name." });
                            }
                            recipeIngredient.IngredientName = ingredientName[i];
                            recipeIngredient.Unit = ingredientUnit[i];
                            if (ingredientQty[i] <= 0 || string.IsNullOrEmpty(Convert.ToString(ingredientQty[i])))
                            {
                                var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                                if (forDeleteIngredients.Count <= 0)
                                {
                                    _recipeRepo.Delete(recipe.RecipeID);
                                    return Json(new { success = false, msg = "Invalid qty input." });
                                }
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "Invalid qty input." });
                            }
                            recipeIngredient.Quantity = ingredientQty[i];
                            if (_recipeIngredientRepo.Create(recipeIngredient) == ErrorCode.Error)
                            {
                                _recipeRepo.Delete(recipe.RecipeID);
                                return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                            }
                        }
                        var sameUserRecipe = _db.UserRecipe.Where(model => model.RecipeID == recipe.RecipeID && model.UserID == user.userID).FirstOrDefault();
                        userRecipe.UserRecipeID = sameUserRecipe.UserRecipeID;
                        userRecipe.RecipeID = recipe.RecipeID;
                        userRecipe.UserID = user.userID;
                        if (_userRecipeRepo.Update(userRecipe.UserRecipeID, userRecipe) == ErrorCode.Error)
                        {
                            _recipeRepo.Delete(recipe.RecipeID);
                            var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                            if (forDeleteIngredients.Count > 0)
                            {
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                            }
                            return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                        }
                        var sameFoodSale = _db.FoodSale.Where(model => model.UserRecipeID == userRecipe.UserRecipeID).FirstOrDefault();
                        foodSale.FoodSaleID = sameFoodSale.FoodSaleID;
                        foodSale.UserRecipeID = userRecipe.UserRecipeID;
                        foodSale.Price = Convert.ToDecimal(price);
                        foodSale.Address = shippedfrom;
                        foodSale.Available = true;
                        foodSale.Stocks = Convert.ToInt32(stock);
                        if (_foodSaleRepo.Update(foodSale.FoodSaleID, foodSale) == ErrorCode.Error)
                        {
                            _recipeRepo.Delete(recipe.RecipeID);
                            var forDeleteIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).ToList();
                            if (forDeleteIngredients.Count > 0)
                            {
                                foreach (var del in forDeleteIngredients)
                                {
                                    _recipeIngredientRepo.Delete(del.RecipeIngredientID);
                                }
                            }
                            return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                        }

                        return Json(new { success = true, msg = "Recipe updated successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, msg = "An error has occured when creating your recipe, please try again." });
                    }
                }
            }
            return Json(new { success = false, msg = "Session timeout." });
        }

        public ActionResult RecipeDetails(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Session["foodSale"] != null)
                {
                    var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
                    var recipe = _recipeRepo.Get(id);
                    var recipeIngredients = _db.RecipeIngredient.Where(m => m.RecipeID == id).ToList();
                    var recipeImage = _db.RecipeImage.Where(m => m.RecipeID == recipe.RecipeID).FirstOrDefault();
                    var userRecipe = _db.UserRecipe.Where(model => model.RecipeID == recipe.RecipeID).FirstOrDefault();
                    if (user.userID == userRecipe.UserID)
                    {
                        var uploaderInfo = _db.User.Where(model => model.userID == userRecipe.UserID).FirstOrDefault();
                        var foodCategory = _db.FoodCategory.ToList();
                        var userPremium = _db.UserPremium.Where(model => model.UserID == uploaderInfo.userID).FirstOrDefault();

                        Session["recipeIngredients"] = recipeIngredients;
                        Session["recipeImage"] = recipeImage;
                        Session["uploaderInfo"] = uploaderInfo;
                        Session["foodCategory"] = foodCategory;
                        //Session["userPremium"] = userPremium;

                        var foodSale = Session["foodSale"] as List<vw_UserRecipeView>;

                        vw_UserRecipeView recipeFoodSle = new vw_UserRecipeView();

                        var editFoodSale = new vw_UserRecipeView();

                        foreach (var item in foodSale)
                        {
                            if (item.RecipeID == id)
                            {
                                editFoodSale = item;
                                break;
                            }
                        }

                        Session["editFoodSale"] = editFoodSale;

                        return View(recipe);
                    } else
                    {
                        return RedirectToAction("../Error/PageNotFound");
                    }

                } else
                {
                    return RedirectToAction("../Account/LogOut");
                }
            }
            return RedirectToAction("../Account/LogOut");
        }

        public ActionResult DeleteRecipe(int id)
        {
            var userRecipe = _db.UserRecipe.Where(model => model.RecipeID == id).FirstOrDefault();
            var recipe = _db.Recipe.Where(model => model.RecipeID == userRecipe.RecipeID).FirstOrDefault();
            if (User.Identity.IsAuthenticated)
            {
                _userRecipeRepo.Delete(userRecipe.UserRecipeID);
                _recipeRepo.Delete(recipe.RecipeID);
                return RedirectToAction("MyUploads");
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        public ActionResult MyFavorites()
        {
            if (User.Identity.IsAuthenticated)
            {
                var recipeDetail = new RecipeDetailViewModel();
                var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
                var myFavorites = _db.vw_MyFavoritesView.Where(model => model.userID == user.userID).ToList();
                recipeDetail.myFavoritesView = myFavorites;
                recipeDetail.faveRecipes = _db.UsersFavoriteRecipes.Where(model => model.UserID == user.userID).Select(model => model.RecipeID).Where(recipeId => recipeId.HasValue).Select(recipeId => recipeId.Value).ToList();

                var isUserPremium = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();
                if (isUserPremium != null)
                {
                    var incomingOrderList = _db.vw_IncomingOrderView.Where(model => model.CustomerID == user.userID).ToList();
                    recipeDetail.incomingOrderView = incomingOrderList;
                }
                else
                {
                    recipeDetail.incomingOrderView = new List<vw_IncomingOrderView>();
                }

                return View(recipeDetail);
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }
    }
}