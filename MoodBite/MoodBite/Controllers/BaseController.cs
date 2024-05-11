using MoodBite.Models.RecipeViewModel;
using MoodBite.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    [HandleError]
    public class BaseController : Controller
    {
        public MoodBiteEntities _db;
        public BaseRepository<Mood> _moodRepo;
        public BaseRepository<User> _userRepo;
        public BaseRepository<UserRole> _userRoleRepo;
        public BaseRepository<UserPremium> _userPremium;
        public BaseRepository<Recipe> _recipeRepo;
        public BaseRepository<UserRecipe> _userRecipeRepo;
        public BaseRepository<RecipeImage> _recipeImageRepo;
        public BaseRepository<RecipeIngredient> _recipeIngredientRepo;
        public BaseRepository<RecipeRating> _recipeRating;
        public BaseRepository<Cart> _cartRepo;
        public BaseRepository<OrderMaster> _orderMasterRepo;
        public BaseRepository<OrderDetail> _orderDetailRepo;
        public BaseRepository<OrderPayment> _orderPaymentRepo;
        public BaseRepository<UsersFavoriteRecipes> _userFaveRecipe;
        public BaseRepository<FoodSale> _foodSaleRepo;
        public BaseRepository<Testimonials> _testimonyRepo;
        public BaseRepository<BuyPremiumTransaction> _buyPremTransactionRepo;

        public BaseController()
        {
            _db = new MoodBiteEntities();
            _moodRepo = new BaseRepository<Mood>();
            _userRepo = new BaseRepository<MoodBite.User>();
            _userRoleRepo = new BaseRepository<UserRole>();
            _userPremium = new BaseRepository<UserPremium>();
            _recipeRepo = new BaseRepository<Recipe>();
            _userRecipeRepo = new BaseRepository<UserRecipe>();
            _recipeImageRepo = new BaseRepository<RecipeImage>();
            _recipeIngredientRepo = new BaseRepository<RecipeIngredient>();
            _recipeRating = new BaseRepository<RecipeRating>();
            _cartRepo = new BaseRepository<Cart>();
            _orderMasterRepo = new BaseRepository<OrderMaster>();
            _orderDetailRepo = new BaseRepository<OrderDetail>();
            _orderPaymentRepo = new BaseRepository<OrderPayment>();
            _userFaveRecipe = new BaseRepository<UsersFavoriteRecipes>();
            _foodSaleRepo = new BaseRepository<FoodSale>();
            _testimonyRepo = new BaseRepository<Testimonials>();
            _buyPremTransactionRepo = new BaseRepository<BuyPremiumTransaction>();
        }

        //function that returns a model that contains all model for Recipe
        public RecipeDetailViewModel RecipeDetail(string chosenMood)
        {
            var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();

            //variable that contains the recommended recipes based on mood inpputed
            var recommendedRecipes = _db.vw_RecommendedRecipeForMood.Where(model => model.MoodName == chosenMood).ToList();

            //variable that contains the ingredients of recipe (raw)
            var recipeDetailsNoIngredients = _db.vw_RecipeDetailsWithoutIngredients.Where(model => model.MoodName == chosenMood).ToList();

            //variable that contains the ingredients of recipe (key: recipe name, value: ingredients)
            var recipeIngredients = new Dictionary<string, List<string>>();

            //variable thatn contains data same with recipeDetailsNoIngredients but with rating
            var recipeRating = _db.vw_RecipeDetailsWithoutIngredientsWithRating.ToList();

            //contains all the model for recipe
            var recipeDetail = new RecipeDetailViewModel();

            //set recommendedRecipes model
            recipeDetail.recommendedRecipes = recommendedRecipes;

            //set recipeDetailsWithoutIngredients model
            recipeDetail.recipeDetailsWithoutIngredients = recipeDetailsNoIngredients;

            //set recipeDetailsWithRating model
            recipeDetail.recipeDetailsWithRating = recipeRating;

            //set allergy model
            recipeDetail.allergy = _db.Allergy.ToList().OrderBy(model => model.AllergyName).Select(model => model.AllergyName);

            //set foodCategories model
            recipeDetail.foodCategories = _db.FoodCategory.OrderBy(model => model.FoodCategoryName).Select(model => model.FoodCategoryName).ToList();

            //set intolorannce model
            recipeDetail.intolerance = _db.Intolerance.ToList().OrderBy(model => model.IntoleranceName).Select(model => model.IntoleranceName);

            //set facerecipe model
            recipeDetail.faveRecipes = _db.UsersFavoriteRecipes.Where(model => model.UserID == user.userID).Select(model => model.RecipeID).Where(recipeId => recipeId.HasValue).Select(recipeId => recipeId.Value).ToList();

            //set incoming recipe modal model
            var isUserPremium = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();
            if (isUserPremium != null)
            {
                var incomingOrderList = _db.vw_IncomingOrderView.Where(model => model.CustomerID == user.userID).ToList();
                recipeDetail.incomingOrderView = incomingOrderList;
            } else
            {
                recipeDetail.incomingOrderView = new List<vw_IncomingOrderView>();
            }

            //var orderMaster = _db.OrderMaster.Where(model => model.CustomerID == user.userID).FirstOrDefault();

            ////set incomingOrderView model
            //recipeDetail.incomingOrderView = _db.vw_IncomingOrderView.Where(model => model.PO_ID == );

            //iterate through recommendedrecipes
            foreach (var recipe in recommendedRecipes)
            {
                //check if the the container for recipeingredients has already have this key to avoid duplication, if key doesnt exist then create the key otherwise ignore
                if (!recipeIngredients.ContainsKey(recipe.RecipeName))
                {
                    //creating key for recipeingredients
                    recipeIngredients[recipe.RecipeName] = new List<string>();

                    //contains the raw table of recipe with ingredients from vw_IngredientsOfRecipe view
                    var recipeIngredientsTable = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.IngredientName }).ToList();

                    //iterate through the table
                    foreach (var ingredient in recipeIngredientsTable)
                    {
                        //for current recipe, add its ingredients
                        recipeIngredients[recipe.RecipeName].Add(ingredient.IngredientName);
                    }
                }
            }

            //set recipeIngredients model
            recipeDetail.recipeIngredients = recipeIngredients;

            //returns the a model that contains all model for Recipe
            return recipeDetail;
        }

        //function that returns a model that contains all model for Recipe //POST
        public RecipeDetailViewModel RecipeDetail(string chosenMood, string search, string[] allergyInp, string[] intoleranceInp, string[] foodCategoryInp)
        {
            List<string> allergyInpList;
            List<string> foodCategoryInpList;
            //variable that contains the recommended recipes based on mood inpputed
            var recommendedRecipes = _db.vw_RecommendedRecipeForMood.Where(model => model.MoodName == chosenMood && model.RecipeName.Contains(search)).ToList();

            if (allergyInp != null && foodCategoryInp != null)
            {
                allergyInpList = allergyInp.ToList();
                foodCategoryInpList = foodCategoryInp.ToList();

                var filteredRecipes = _db.sp_FilterSearchByAllergyAndFoodCat(chosenMood, search, string.Join(",", allergyInpList), string.Join(",", foodCategoryInpList))
                    .AsEnumerable()
                    .Select(result => new vw_RecommendedRecipeForMood
                    {
                        RecipeID = result.RecipeID,
                        RecipeName = result.RecipeName,
                        FoodCategoryName = result.FoodCategoryName,
                        MoodName = result.MoodName,
                        userID = result.userID,
                        Uploaded_by = result.Uploaded_by,
                        DateUploaded = result.DateUploaded,
                        Approved_by = result.Approved_by,
                        DateApproved = result.DateApproved,
                        RecipeImageID = result.RecipeImageID,
                        ImageName = result.ImageName,
                        ImagePath = result.ImagePath
                    })
                    .ToList();

                recommendedRecipes = filteredRecipes;
            } else
            {
                allergyInpList = null;
                foodCategoryInpList = null;
            }

            if (allergyInp != null)
            {
                allergyInpList = allergyInp.ToList();
                var filteredRecipes = _db.sp_FilterSearchByAllergy(chosenMood, search, string.Join(",", allergyInpList))
                .AsEnumerable()
                .Select(result => new vw_RecommendedRecipeForMood
                {
                    RecipeID = result.RecipeID,
                    RecipeName = result.RecipeName,
                    FoodCategoryName = result.FoodCategoryName,
                    MoodName = result.MoodName,
                    userID = result.userID,
                    Uploaded_by = result.Uploaded_by,
                    DateUploaded = result.DateUploaded,
                    Approved_by = result.Approved_by,
                    DateApproved = result.DateApproved,
                    RecipeImageID = result.RecipeImageID,
                    ImageName = result.ImageName,
                    ImagePath = result.ImagePath
                })
                .ToList();

                recommendedRecipes = filteredRecipes;
            }
            else
            {
                allergyInpList = null;
            }

            if (foodCategoryInp != null)
            {
                foodCategoryInpList = foodCategoryInp.ToList();
                var filteredRecipes = _db.sp_FilterSearchByFoodCat(chosenMood, search, string.Join(",", foodCategoryInpList))
                .AsEnumerable()
                .Select(result => new vw_RecommendedRecipeForMood
                {
                    RecipeID = result.RecipeID,
                    RecipeName = result.RecipeName,
                    FoodCategoryName = result.FoodCategoryName,
                    MoodName = result.MoodName,
                    userID = result.userID,
                    Uploaded_by = result.Uploaded_by,
                    DateUploaded = result.DateUploaded,
                    Approved_by = result.Approved_by,
                    DateApproved = result.DateApproved,
                    RecipeImageID = result.RecipeImageID,
                    ImageName = result.ImageName,
                    ImagePath = result.ImagePath
                })
                .ToList();

                recommendedRecipes = filteredRecipes;
            }
            else
            {
                foodCategoryInpList = null;
            }

            //variable that contains the ingredients of recipe (raw)
            var recipeDetailsNoIngredients = _db.vw_RecipeDetailsWithoutIngredients.Where(model => model.MoodName == chosenMood && model.RecipeName.Contains(search)).ToList();

            //variable that contains the ingredients of recipe (key: recipe name, value: ingredients)
            var recipeIngredients = new Dictionary<string, List<string>>();

            //variable thatn contains data same with recipeDetailsNoIngredients but with rating
            var recipeRating = _db.vw_RecipeDetailsWithoutIngredientsWithRating.ToList();

            //contains all the model for recipe
            var recipeDetail = new RecipeDetailViewModel();

            //set recommendedRecipes model
            recipeDetail.recommendedRecipes = recommendedRecipes;

            //set recipeDetailsWithoutIngredients model
            recipeDetail.recipeDetailsWithoutIngredients = recipeDetailsNoIngredients;

            //set recipeDetailsWithRating model
            recipeDetail.recipeDetailsWithRating = recipeRating;

            //set allergy model
            recipeDetail.allergy = _db.Allergy.ToList().OrderBy(model => model.AllergyName).Select(model => model.AllergyName);

            //set foodCategories model
            recipeDetail.foodCategories = _db.FoodCategory.OrderBy(model => model.FoodCategoryName).Select(model => model.FoodCategoryName).ToList();

            //set intolorannce model
            recipeDetail.intolerance = _db.Intolerance.ToList().OrderBy(model => model.IntoleranceName).Select(model => model.IntoleranceName);

            //set facerecipe model
            recipeDetail.faveRecipes = _db.UsersFavoriteRecipes.Select(model => model.RecipeID).Where(recipeId => recipeId.HasValue).Select(recipeId => recipeId.Value).ToList();

            //set incoming recipe modal model
            var user = _db.User.Where(model => model.Username == User.Identity.Name).FirstOrDefault();
            var isUserPremium = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();
            if (isUserPremium != null)
            {
                var incomingOrderList = _db.vw_IncomingOrderView.Where(model => model.CustomerID == user.userID).ToList();
                recipeDetail.incomingOrderView = incomingOrderList;
            }

            //iterate through recommendedrecipes
            foreach (var recipe in recommendedRecipes)
            {
                //check if the the container for recipeingredients has already have this key to avoid duplication, if key doesnt exist then create the key otherwise ignore
                if (!recipeIngredients.ContainsKey(recipe.RecipeName))
                {
                    //creating key for recipeingredients
                    recipeIngredients[recipe.RecipeName] = new List<string>();

                    //contains the raw table of recipe with ingredients from vw_IngredientsOfRecipe view
                    var recipeIngredientsTable = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.IngredientName }).ToList();

                    //iterate through the table
                    foreach (var ingredient in recipeIngredientsTable)
                    {
                        //for current recipe, add its ingredients
                        recipeIngredients[recipe.RecipeName].Add(ingredient.IngredientName);
                    }
                }
            }

            //set recipeIngredients model
            recipeDetail.recipeIngredients = recipeIngredients;

            //returns the a model that contains all model for Recipe
            return recipeDetail;
        }

        public RecipeDetailViewModel ReadMore(int id)
        {
            try
            {
                //contains the details of recipe that will be rendered in RecipeReadMore
                var recipeDetail = new RecipeDetailViewModel();

                //contains the recipe for read more
                var recipe = _db.vw_RecommendedRecipeForMood.Where(model => model.RecipeID == id).FirstOrDefault();

                //uploadersprofile info
                var userProfile = _db.vw_AllUserColWithRecipeID.Where(model => model.RecipeID == id).FirstOrDefault();

                //contains the images of recipe for read more
                var recipeImages = _db.vw_CoverImageOfRecipes.Where(model => model.RecipeID == id).FirstOrDefault();

                //contains the ingredients of recipe for read more
                var recipeIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.IngredientName, model.Quantity, model.Unit }).ToList();

                //contains the recipe details without ingredients for read more
                var recipeDetailsWithoutIngredients = _db.vw_RecipeDetailsWithoutIngredients.Where(model => model.RecipeID == id).FirstOrDefault();

                //chosen mood data from Session
                var chosenMood = Session["ChosenMood"] as String;

                //contains all the images of recipe
                var allRecipeImages = _db.vw_CoverImageOfRecipes.Where(model => model.MoodName == chosenMood && model.RecipeID != id).ToList();

                //contains all the recipes
                var allRecommendedRecipes = _db.vw_RecommendedRecipeForMood.Where(model => model.MoodName == chosenMood && model.RecipeID != id).ToList();

                //contains all recipe ingredients
                var allRecipeIngredients = new Dictionary<string, List<string>>();

                //contains the ratings of recipe
                var recipeRating = _db.vw_RecipeDetailsWithoutIngredientsWithRating.ToList();

                //contains all the recipe uploaders details with recipe rating 
                var recipeUploadersNameWithRatingReadMore = _db.vw_recipeUploadersNameWithRating.ToList();

                //contains the total recipe uploads of the uploader
                var userUploadCounts = _db.vw_UsersUploadCounts.ToList();

                //set recipeReadMore model
                recipeDetail.recipeReadMore = recipe;

                //set recipeImagesReadMore model
                recipeDetail.recipeImagesReadMore = recipeImages;

                //set recipeIngredientsReadMore model
                recipeDetail.recipeIngredientsReadMore = recipeIngredients.Select(model => new RecipeIngredient { IngredientName = model.IngredientName, Unit = model.Unit, Quantity = model.Quantity }).ToList();

                //set recipeDetailsWithoutIngredientsReadMore model
                recipeDetail.recipeDetailsWithoutIngredientsReadMore = recipeDetailsWithoutIngredients;

                //set recipeDetailsWithRating model
                recipeDetail.recipeDetailsWithRating = recipeRating;

                //set recipeUploadersNameWithRatingReadMore model
                recipeDetail.recipeUploadersNameWithRatingReadMore = recipeUploadersNameWithRatingReadMore;

                //set userUploadCounts model
                recipeDetail.userUploadCounts = userUploadCounts;

                //set uploadersProfilePic mode/
                recipeDetail.uploadersProfilePic = userProfile;

                //set allRecipeWithFoodCategoryName
                recipeDetail.allRecipeWithFoodCategoryName = _db.vw_AllRecipeDetailsWithFoodCategoryName.ToList();

                //set allergy model
                recipeDetail.allergy = _db.Allergy.ToList().Select(model => model.AllergyName);

                //set foodCategories
                recipeDetail.foodCategories = _db.FoodCategory.Select(model => model.FoodCategoryName).ToList();

                //set intolorannce
                recipeDetail.intolerance = _db.Intolerance.ToList().Select(model => model.IntoleranceName);

                //set userpremium model
                var user = Session["User"] as User;
                recipeDetail.userPremium = _db.UserPremium.Where(model => model.UserID == user.userID).FirstOrDefault();

                //set foodSale model
                recipeDetail.foodSaleView = _db.vw_FoodSaleView.Where(model => model.RecipeID == id).FirstOrDefault();

                //set recipereadmore stocks view
                //recipeDetail.recipeStocksView = _db.vw_RecipeReadMoreStocksView.Where(model => model.FoodSaleID == )

                //check if theres available recommended recipes
                if (allRecommendedRecipes.Count != 0)
                {
                    //iterate through available recommended recipes model
                    foreach (var recomRecipe in allRecommendedRecipes)
                    {
                        //check if the the container for recipeingredients has already have this key to avoid duplication, if key doesnt exist then create the key otherwise ignore
                        if (!allRecipeIngredients.ContainsKey(recomRecipe.RecipeName))
                        {
                            //creating key for recipeingredients
                            allRecipeIngredients[recomRecipe.RecipeName] = new List<string>();

                            //SELECT ingredientname from recipeingredients where recipeid = recommeneded recipeid
                            var recipeIngredientsTable = _db.RecipeIngredient.Where(model => model.RecipeID == recomRecipe.RecipeID).Select(model => new { model.IngredientName }).ToList();

                            //iterate through the table
                            foreach (var ingredient in recipeIngredientsTable)
                            {
                                //for current recipe, add its ingredients
                                allRecipeIngredients[recomRecipe.RecipeName].Add(ingredient.IngredientName);
                            }
                        }
                    }

                    //set imagesOfRecipe model
                    recipeDetail.imagesOfRecipe = allRecipeImages;

                    //set recommendedRecipes model
                    recipeDetail.recommendedRecipes = allRecommendedRecipes;

                    //set recipeIngredients model
                    recipeDetail.recipeIngredients = allRecipeIngredients;
                }
                else
                {
                    //if theres no available recommended recipe then display appropriate message
                    ViewBag.NoRecipesWithSimilarMoodTagMsg = "Out of recipe...";
                }
                return recipeDetail;

            }
            catch (Exception)
            {
                ViewBag.NoRecipesWithSimilarMoodTagMsg = "Out of recipe...";
                return null;
            }
        }

        public string GetUniqueFileName(string parentPath, string fileName)
        {
            string uniqueName = fileName;
            int count = 1;
            string filePath = Path.Combine(Server.MapPath(parentPath), uniqueName);

            while (System.IO.File.Exists(filePath))
            {
                // If the file already exists, append a number to the filename
                string extension = Path.GetExtension(fileName);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                uniqueName = $"{fileNameWithoutExtension}_{count}{extension}";
                filePath = Path.Combine(Server.MapPath(parentPath), uniqueName);
                count++;
            }

            return uniqueName;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new EmailAddressAttribute();
                return addr.IsValid(email);
            }
            catch
            {
                return false;
            }
        }

    }
}