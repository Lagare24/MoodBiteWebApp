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
        //[Authorize(Roles = "User, Admin")]
        //public ActionResult UsersHome(string mood)
        //{
        //    var recommendedRecipes = _db.vw_RecommendedRecipeForMood.Where(model => model.MoodName == mood).ToList();
        //    var recipeDetailsNoIngredients = _db.vw_RecipeDetailsWithoutIngredients.Where(model => model.MoodName == mood).ToList();
        //    Dictionary<string, List<string>> recipeIngredients = new Dictionary<string, List<string>>();

        //    RecipeDetailViewModel recipeDetail = new RecipeDetailViewModel();
        //    recipeDetail.recommendedRecipes = recommendedRecipes;
        //    recipeDetail.recipeDetailsWithoutIngredients = recipeDetailsNoIngredients;

        //    foreach (var recipe in recommendedRecipes)
        //    {
        //        if(!recipeIngredients.ContainsKey(recipe.RecipeName))
        //        {
        //            recipeIngredients[recipe.RecipeName] = new List<string>();
        //            var recipeIngredientsTable = _db.vw_IngredientsOfRecipe.Where(model => model.RecipeID == recipe.RecipeID).Select(model => new { model.IngredientName }).ToList();
        //            foreach (var ingredient in recipeIngredientsTable)
        //            {
        //                recipeIngredients[recipe.RecipeName].Add(ingredient.IngredientName);
        //            }
        //        }
        //    }
        //    recipeDetail.recipeIngredients = recipeIngredients;
        //    return View(recipeDetail);
        //}

        public ActionResult ChooseMood()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChooseMood(string chosenMood)
        {
            Session["ChosenMood"] = chosenMood;
            //return RedirectToAction("UsersHome");
            return RedirectToAction("UsersHome");
        }

        public ActionResult UsersHome()
        {
            if (User.Identity.IsAuthenticated)
            {
                var chosenMood = Session["ChosenMood"] as String;
                var user = Session["User"] as User;

                return View();
            } else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        public RecipeDetailViewModel RecipeDetail(string chosenMood)
        {
            var recommendedRecipes = _db.vw_RecommendedRecipeForMood.Where(model => model.MoodName == chosenMood).ToList();
            var recipeDetailsNoIngredients = _db.vw_RecipeDetailsWithoutIngredients.Where(model => model.MoodName == chosenMood).ToList();
            var recipeIngredients = new Dictionary<string, List<string>>();

            var recipeDetail = new RecipeDetailViewModel();
            recipeDetail.recommendedRecipes = recommendedRecipes;
            recipeDetail.recipeDetailsWithoutIngredients = recipeDetailsNoIngredients;

            return recipeDetail;
        }
    }
}