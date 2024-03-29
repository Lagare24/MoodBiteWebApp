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
        public BaseRepository<Recipe> _recipeRepo;
        public BaseRepository<vw_IngredientsOfRecipe> _recipeIngredientsRepo;
        public BaseRepository<UserRole> _userRoleRepo;

        public BaseController()
        {
            _db = new MoodBiteEntities();
            _moodRepo = new BaseRepository<Mood>();
            _userRepo = new BaseRepository<MoodBite.User>();
            _recipeRepo = new BaseRepository<Recipe>();
            _recipeIngredientsRepo = new BaseRepository<vw_IngredientsOfRecipe>();
            _userRoleRepo = new BaseRepository<UserRole>();
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
                    var recipeIngredientsTable = _db.vw_IngredientsOfRecipe.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.Ingredient }).ToList();

                    //iterate through the table
                    foreach (var ingredient in recipeIngredientsTable)
                    {
                        //for current recipe, add its ingredients
                        recipeIngredients[recipe.RecipeName].Add(ingredient.Ingredient);
                    }
                }
            }

            //set recipeIngredients model
            recipeDetail.recipeIngredients = recipeIngredients;

            //returns the a model that contains all model for Recipe
            return recipeDetail;
        }


    }
}