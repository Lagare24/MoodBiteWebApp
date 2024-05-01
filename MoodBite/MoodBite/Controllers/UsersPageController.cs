using MoodBite.Models.CartModel;
using MoodBite.Models.RecipeViewModel;
using MoodBite.Models.UploadRecipeModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    [HandleError]
    [Authorize(Roles = "User, Admin")]
    [OutputCache(NoStore = true, Duration = 0)]
    public class UsersPageController : BaseController
    {
        public string localChosenMood;
        //modal pop up which prompts the user to select a mood
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ChooseMood()
        {
            if(User.Identity.IsAuthenticated)
            {
                var moodList = _db.Mood.ToList();
                return View(moodList);
            }
            return RedirectToAction("../Account/LogIn");
        }

        //handles the submission for mood selection
        [HttpPost]
        public ActionResult ChooseMood(string chosenMood)
        {
            Session["ChosenMood"] = chosenMood;
            this.localChosenMood = chosenMood;
            //return RedirectToAction("UsersHome");
            return RedirectToAction("UsersHome");
        }

        //after selecting mood user will be redirected here, where recipes will be generated based on mood inputted
        public ActionResult UsersHome()
        {
            if(Session["ChosenMood"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            }
            else if (User.Identity.IsAuthenticated && Session["ChosenMood"] != null)
            {
                var chosenMood = Session["ChosenMood"].ToString();
                var user = Session["User"] as User;
                var recipedetail = RecipeDetail(chosenMood);
                Session["SearchInput"] = string.Empty;
                return View(recipedetail);
            }
            else
            {
                return RedirectToAction("../Home/Index");
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
                    Session["SearchInput"] = search;
                    var recipedetail = RecipeDetail(chosenMood);
                    return View(recipedetail);
                } else
                {
                    var chosenMood = Session["ChosenMood"] as String;
                    var user = Session["User"] as User;
                    Session["SearchInput"] = search;
                    var recipedetail = RecipeDetail(chosenMood, search, allergyInp, intoleranceInp, foodCategoryInp);
                    return View(recipedetail);
                }
            }
            else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        //view full details when read more button is clicked from the recipe card/item
        public ActionResult RecipeReadMore(int id)
        {

            if (Session["ChosenMood"] == null || Session["User"] == null)
            {
                return RedirectToAction("../Account/LogOut");
            }
            else if(User.Identity.IsAuthenticated && Session["ChosenMood"] != null && Session["User"] != null)
            {
                var recipeDetail = ReadMore(id);
                var user = Session["User"] as User;
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
                Session["UserPremium"] = userPremium;
                return View(model);
            }
            else
            {
                return RedirectToAction("../Account/LogOuts");
            }
           
        }

        //[HttpPost]
        //public ActionResult UploadRecipe(Recipe recipe, string ingcount, string moodid, string[] ingredientName, double[] ingredientQty, string[] ingredientUnit)
        //{

        //    if (User.Identity.IsAuthenticated && User.Identity.Name != null)
        //    {
        //        var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
        //        var imageFile = Request.Files.Get("imageFile");

        //        var uploadRecipe = new Recipe();
        //        uploadRecipe.RecipeID = recipe.RecipeID;
        //        uploadRecipe.RecipeName = recipe.RecipeName;
        //        uploadRecipe.RecipeDescription = recipe.RecipeDescription;
        //        uploadRecipe.PreparationTime = TimeSpan.Parse(recipe.PreparationTime.ToString());
        //        uploadRecipe.CookingDuration = TimeSpan.Parse(recipe.CookingDuration.ToString());
        //        uploadRecipe.DateUploaded = DateTime.Today;
        //        uploadRecipe.CookingInstruction = recipe.CookingInstruction;
        //        uploadRecipe.IngredientsCount = Convert.ToInt32(ingcount);
        //        uploadRecipe.IsApproved = false;
        //        uploadRecipe.MoodID = Convert.ToInt32(moodid);
        //        _recipeRepo.Create(uploadRecipe);

        //        int newRecipeID = uploadRecipe.RecipeID;

        //        if (imageFile != null && imageFile.ContentLength > 0)
        //        {
        //            var recipeImage = new RecipeImage();
        //            recipeImage.RecipeID = newRecipeID;
        //            recipeImage.ImageName = recipe.RecipeName + " cover";

        //            using (var binaryReader = new BinaryReader(imageFile.InputStream))
        //            {
        //                recipeImage.ImageURL = binaryReader.ReadBytes(imageFile.ContentLength);
        //            }
        //            try
        //            {
        //                _recipeImageRepo.Create(recipeImage);
        //            }
        //            catch (Exception)
        //            {
        //                _recipeRepo.Delete(newRecipeID);
        //            }
        //        }

        //        var userRecipe = new UserRecipe();
        //        userRecipe.UserID = user.userID;
        //        userRecipe.RecipeID = newRecipeID;
        //        try
        //        {
        //            _userRecipeRepo.Create(userRecipe);
        //            int newUserRecipeID = userRecipe.UserRecipeID;

        //            var recipeIngredients = new RecipeIngredient();
        //            for (int i = 0; i < Convert.ToInt32(ingcount); i++)
        //            {
        //                recipeIngredients.RecipeID = newRecipeID;
        //                recipeIngredients.IngredientName = ingredientName[i];
        //                recipeIngredients.Unit = ingredientUnit[i];
        //                recipeIngredients.Quantity = ingredientQty[i];
        //                _recipeIngredientRepo.Create(recipeIngredients);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            _recipeImageRepo.Delete(newRecipeID);
        //        }
        //        return RedirectToAction("UsersHome");
        //    }
        //    return View();
        //}

        [HttpPost]
        public ActionResult UploadRecipe(Recipe recipe, string ingcount, string moodid, string[] ingredientName, double[] ingredientQty, string[] ingredientUnit, double? price, string shippedfrom)
        {

            if (User.Identity.IsAuthenticated && User.Identity.Name != null)
            {
                var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();

                var uploadRecipe = new Recipe();
                uploadRecipe.RecipeID = recipe.RecipeID;
                uploadRecipe.RecipeName = recipe.RecipeName;
                uploadRecipe.RecipeDescription = recipe.RecipeDescription;
                uploadRecipe.PreparationTime = TimeSpan.Parse(recipe.PreparationTime.ToString());
                uploadRecipe.CookingDuration = TimeSpan.Parse(recipe.CookingDuration.ToString());
                uploadRecipe.DateUploaded = DateTime.Today;
                uploadRecipe.CookingInstruction = recipe.CookingInstruction;
                uploadRecipe.IngredientsCount = Convert.ToInt32(ingcount);
                uploadRecipe.IsApproved = false;
                uploadRecipe.MoodID = Convert.ToInt32(moodid);
                _recipeRepo.Create(uploadRecipe);

                int newRecipeID = uploadRecipe.RecipeID;

                var imageFile = Request.Files.Get("imageFile");
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var recipeImage = new RecipeImage();
                    recipeImage.RecipeID = newRecipeID;
                    recipeImage.ImageName = recipe.RecipeName + " cover";

                    string fileName = Path.GetFileName(imageFile.FileName);
                    string uniqueFileName = GetUniqueFileName("~/Content/RecipeImages/", fileName);
                    string filePath = Path.Combine(Server.MapPath("~/Content/RecipeImages/"), uniqueFileName);

                    //Save the file to the specified path
                    imageFile.SaveAs(filePath);

                    // Store the file path in the database
                    recipeImage.ImagePath = "~/Content/RecipeImages/" + uniqueFileName;

                    try
                    {
                        _recipeImageRepo.Create(recipeImage);
                    }
                    catch (Exception)
                    {
                        _recipeRepo.Delete(newRecipeID);
                        return View();
                    }

                    //using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    //{
                    //    recipeImage.ImageURL = binaryReader.ReadBytes(imageFile.ContentLength);
                    //}
                    //try
                    //{
                    //    _recipeImageRepo.Create(recipeImage);
                    //}
                    //catch (Exception)
                    //{
                    //    _recipeRepo.Delete(newRecipeID);
                    //}

                    //var profilePicture = Request.Files.Get("profilePic");

                    //if (profilePicture != null && profilePicture.ContentLength > 0)
                    //{
                    //    string fileName = Path.GetFileName(profilePicture.FileName);
                    //    string uniqueFileName = GetUniqueFileName(fileName);
                    //    string filePath = Path.Combine(Server.MapPath("~/Content/UsersProfileImages/"), uniqueFileName);

                    //    // Save the file to the specified path
                    //    profilePicture.SaveAs(filePath);

                    //    // Store the file path in the database
                    //    u.ProfilePicturePath = "~/Content/UsersProfileImages/" + uniqueFileName;
                    //    try
                    //    {
                    //        _userRepo.Update(user.userID, u);
                    //        return Json(new { success = true, message = "Profile updated successfully" });
                    //    }
                    //    catch (Exception)
                    //    {
                    //        return Json(new { success = false, message = "An error has occured" });
                    //    }

                    //}
                    //else
                    //{
                    //    return Json(new { success = false, message = "An error has occured" });
                    //}
                }

                var userRecipe = new UserRecipe();
                userRecipe.UserID = user.userID;
                userRecipe.RecipeID = newRecipeID;
                try
                {
                    _userRecipeRepo.Create(userRecipe);
                    int newUserRecipeID = userRecipe.UserRecipeID;

                    var recipeIngredients = new RecipeIngredient();
                    for (int i = 0; i < Convert.ToInt32(ingcount); i++)
                    {
                        recipeIngredients.RecipeID = newRecipeID;
                        recipeIngredients.IngredientName = ingredientName[i];
                        recipeIngredients.Unit = ingredientUnit[i];
                        recipeIngredients.Quantity = ingredientQty[i];
                        _recipeIngredientRepo.Create(recipeIngredients);
                    }
                    if ((price > 0 || price != null) && !string.IsNullOrEmpty(shippedfrom))
                    {
                        var foodSale = new FoodSale();
                        foodSale.UserRecipeID = newUserRecipeID;
                        foodSale.Price = Convert.ToDecimal(price);
                        foodSale.Address = shippedfrom;
                        foodSale.Available = true;
                        try
                        {
                            _foodSaleRepo.Create(foodSale);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                catch (Exception)
                {
                    _recipeImageRepo.Delete(newRecipeID);
                }
                return RedirectToAction("UsersHome");
            }
            return View();
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
                var myCartItem = _db.vw_CartView.Where(model => model.userID == user.userID).ToList();
                return View(myCartItem);
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
                cart.Qty = 0;
                try
                {
                    var itemExist = _db.Cart.Where(model => model.RecipeID == recipeID).FirstOrDefault();

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
            if (Session["User"] != null)
            {
                var cart = _db.vw_CheckOutView.Where(model => model.userID == id).ToList(); 
                return View(cart);
            } else
            {
                return RedirectToAction("../Account/LogOut");
            }
        }

        [HttpPost]
        //id = userid
        public ActionResult PlaceOrder(int id, string address, string paymentMethod, decimal totalAmount)
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
                    _orderMasterRepo.Create(om);

                    var omId = om.PO_ID;

                    OrderDetail od = new OrderDetail();

                    foreach (var item in cart)
                    {
                        od.PO_ID = omId;
                        od.FoodForSaleID = item.FoodSaleID;
                        od.Quantity = item.Qty;
                        od.UnitPrice = item.Price;
                        od.TotalPrice = item.Total_Price;

                        _orderDetailRepo.Create(od);
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

        public ActionResult MyFavorites()
        {
            return View();
        }
    }
}