using MoodBite.Models.RecipeViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class UsersPageController : BaseController
    {
        public string chosenMood;
        //modal pop up which prompts the user to select a mood
        public ActionResult ChooseMood()
        {
            return View();
        }

        //handles the submission for mood selection
        [HttpPost]
        public ActionResult ChooseMood(string chosenMood)
        {
            Session["ChosenMood"] = chosenMood;
            //return RedirectToAction("UsersHome");
            return RedirectToAction("UsersHome");
        }

        //after selecting mood user will be redirected here, where recipes will be generated based on mood inputted
        public ActionResult UsersHome()
        {
            if (User.Identity.IsAuthenticated)
            {
                var chosenMood = Session["ChosenMood"] as String;
                var user = Session["User"] as User;
                var recipedetail = RecipeDetail(chosenMood);


                return View(recipedetail);
            } else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        //view full details when read more button is clicked from the recipe card/item
        public ActionResult RecipeReadMore(int id)
        {
            //contains the details of recipe that will be rendered in RecipeReadMore
            var recipeDetail = new RecipeDetailViewModel();
            var recipe = _db.vw_RecommendedRecipeForMood.Where(model => model.RecipeID == id).FirstOrDefault();
            var recipeImages = _db.vw_CoverImageOfRecipes.Where(model => model.RecipeID == id).FirstOrDefault();
            var recipeIngredients = _db.vw_IngredientsOfRecipe.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.Ingredient }).ToList();
            var varrecipeDetailsWithoutIngredients = _db.vw_RecipeDetailsWithoutIngredients.Where(model => model.RecipeID == id).FirstOrDefault();


            var chosenMood = Session["ChosenMood"] as String;
            var allRecipeImages = _db.vw_CoverImageOfRecipes.Where(model => model.MoodName == chosenMood && model.RecipeID != id).ToList();
            var allRecommendedRecipes = _db.vw_RecommendedRecipeForMood.Where(model => model.MoodName == chosenMood && model.RecipeID != id).ToList();
            var allRecipeIngredients = new Dictionary<string, List<string>>();

            var recipeRating = _db.vw_RecipeDetailsWithoutIngredientsWithRating.ToList();

            recipeDetail.recipeReadMore = recipe;
            recipeDetail.recipeImagesReadMore = recipeImages;
            recipeDetail.recipeIngredientsReadMore = recipeIngredients.Select(model => new vw_IngredientsOfRecipe { Ingredient = model.Ingredient }).ToList();
            recipeDetail.recipeDetailsWithoutIngredientsReadMore = varrecipeDetailsWithoutIngredients;

            recipeDetail.recipeDetailsWithRating = recipeRating;

            if (allRecommendedRecipes.Count != 0)
            {
                foreach (var recomRecipe in allRecommendedRecipes)
                {
                    //check if the the container for recipeingredients has already have this key to avoid duplication, if key doesnt exist then create the key otherwise ignore
                    if (!allRecipeIngredients.ContainsKey(recomRecipe.RecipeName))
                    {
                        //creating key for recipeingredients
                        allRecipeIngredients[recomRecipe.RecipeName] = new List<string>();

                        //contains the raw table of recipe with ingredients from vw_IngredientsOfRecipe view
                        var recipeIngredientsTable = _db.vw_IngredientsOfRecipe.Where(model => model.RecipeID == recomRecipe.RecipeID).Select(model => new { model.Ingredient }).ToList();

                        //iterate through the table
                        foreach (var ingredient in recipeIngredientsTable)
                        {
                            //for current recipe, add its ingredients
                            allRecipeIngredients[recomRecipe.RecipeName].Add(ingredient.Ingredient);
                        }
                    }
                }
                recipeDetail.imagesOfRecipe = allRecipeImages;
                recipeDetail.recommendedRecipes = allRecommendedRecipes;
                recipeDetail.recipeIngredients = allRecipeIngredients;
            } else
            {
                ViewBag.NoRecipesWithSimilarMoodTagMsg = "Out of recipe...";
            }

            return View(recipeDetail);
        }
    }
}