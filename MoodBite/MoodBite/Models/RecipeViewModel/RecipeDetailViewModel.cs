using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoodBite.Models.RecipeViewModel
{
    public class RecipeDetailViewModel
    {
        public IEnumerable<vw_RecipeDetailsWithoutIngredients> recipeDetailsWithoutIngredients;
        public IEnumerable<vw_RecommendedRecipeForMood> recommendedRecipes;
        public Dictionary<string, List<string>> recipeIngredients;
    }
}