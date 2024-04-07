using MoodBite.Models.RecipeViewModel;
using MoodBite.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    public class BaseController : Controller
    {
        public MoodBiteEntities _db;
        public BaseRepository<Mood> _moodRepo;
        public BaseRepository<User> _userRepo;
        public BaseRepository<UserRole> _userRoleRepo;
        public BaseRepository<UserPremium> _userPremiumRecipe;
        public BaseRepository<Recipe> _recipeRepo;
        public BaseRepository<UserRecipe> _userRecipeRepo;
        public BaseRepository<RecipeImage> _recipeImageRepo;
        public BaseRepository<RecipeIngredient> _recipeIngredientRepo;

        public BaseController()
        {
            _db = new MoodBiteEntities();
            _moodRepo = new BaseRepository<Mood>();
            _userRepo = new BaseRepository<MoodBite.User>();
            _userRoleRepo = new BaseRepository<UserRole>();
            _userPremiumRecipe = new BaseRepository<UserPremium>();
            _recipeRepo = new BaseRepository<Recipe>();
            _userRecipeRepo = new BaseRepository<UserRecipe>();
            _recipeImageRepo = new BaseRepository<RecipeImage>();
            _recipeIngredientRepo = new BaseRepository<RecipeIngredient>();
        }

        //function that returns a model that contains all model for Recipe
        public RecipeDetailViewModel RecipeDetail(string chosenMood)
        {
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
            //contains the details of recipe that will be rendered in RecipeReadMore
            var recipeDetail = new RecipeDetailViewModel();

            //contains the recipe for read more
            var recipe = _db.vw_RecommendedRecipeForMood.Where(model => model.RecipeID == id).FirstOrDefault();

            //contains the images of recipe for read more
            var recipeImages = _db.vw_CoverImageOfRecipes.Where(model => model.RecipeID == id).FirstOrDefault();

            //contains the ingredients of recipe for read more
            var recipeIngredients = _db.RecipeIngredient.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.IngredientName }).ToList();

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
            recipeDetail.recipeIngredientsReadMore = recipeIngredients.Select(model => new RecipeIngredient { IngredientName = model.IngredientName }).ToList();

            //set recipeDetailsWithoutIngredientsReadMore model
            recipeDetail.recipeDetailsWithoutIngredientsReadMore = recipeDetailsWithoutIngredients;

            //set recipeDetailsWithRating model
            recipeDetail.recipeDetailsWithRating = recipeRating;

            //set recipeUploadersNameWithRatingReadMore model
            recipeDetail.recipeUploadersNameWithRatingReadMore = recipeUploadersNameWithRatingReadMore;

            //set userUploadCounts model
            recipeDetail.userUploadCounts = userUploadCounts;

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

    }
}