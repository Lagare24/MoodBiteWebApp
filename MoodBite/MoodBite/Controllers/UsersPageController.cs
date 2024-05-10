using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Rotativa;
using MoodBite.Models.RecipeViewModel;
using System.Net.Http.Headers;
using System.Collections.Generic;
using MoodBite.Models.FoodSaleModel;

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
        public ActionResult UploadRecipe(Recipe recipe, string foodcategory, string ingcount, string moodid, string[] ingredientName, double[] ingredientQty, string[] ingredientUnit, double? price, string shippedfrom, string stock)
        {

            if (User.Identity.IsAuthenticated && User.Identity.Name != null)
            {
                var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();

                var uploadRecipe = new Recipe();
                //uploadRecipe.RecipeID = recipe.RecipeID;
                uploadRecipe.RecipeName = recipe.RecipeName;
                uploadRecipe.FoodCategoryID = Convert.ToInt32(foodcategory);
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
                        return Json(new { success = false, msg = "An error has occured." });

                    }
                }

                var userRecipe = new UserRecipe();
                userRecipe.UserID = user.userID;
                userRecipe.RecipeID = newRecipeID;
                try
                {
                    _userRecipeRepo.Create(userRecipe);
                    int newUserRecipeID = userRecipe.UserRecipeID;

                    var recipeIngredients = new RecipeIngredient();
                    for (int i = 1; i < Convert.ToInt32(ingcount)+1; i++)
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
                        foodSale.Stocks = Convert.ToInt32(stock);
                        foodSale.Available = true;
                        try
                        {
                            _foodSaleRepo.Create(foodSale);
                        }
                        catch (Exception)
                        {
                            return Json(new { success = false, msg = "An error has occured." });

                            throw;
                        }
                    }
                }
                catch (Exception)
                {
                    _recipeImageRepo.Delete(newRecipeID);
                    return Json(new { success = false, msg = "An error has occured." });

                }
                return Json(new { success = true, msg = "Your recipe has been successfully uploaded. It is currently awaiting approval from the administrators before it can be officially published." });
            } else
            {
                return Json(new { success = false, msg = "An error has occured." });
            }
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

            if (user.userID == id)
            {
                if (Session["User"] != null)
                {
                    var cart = _db.vw_CheckOutView.Where(model => model.userID == id).ToList();
                    return View(cart);
                }
                else
                {
                    return RedirectToAction("../Account/LogOut");
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

        public ActionResult MyFavorites()
        {
            return View();
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
        public ActionResult EditRecipe(Recipe recipe, string foodcategory, string ingcount, string moodid, string[] ingredientName, int[] ingredientQty, string[] ingredientUnit, int stock)
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
    }
}