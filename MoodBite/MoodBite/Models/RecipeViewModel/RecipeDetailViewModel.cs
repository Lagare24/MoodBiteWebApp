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
        public IEnumerable<vw_CoverImageOfRecipes> imagesOfRecipe;
        public vw_CoverImageOfRecipes recipeImagesReadMore;
        public vw_RecommendedRecipeForMood recipeReadMore;
        public IEnumerable<RecipeIngredient> recipeIngredientsReadMore;
        public vw_RecipeDetailsWithoutIngredients recipeDetailsWithoutIngredientsReadMore;
        public IEnumerable<vw_RecipeDetailsWithoutIngredientsWithRating> recipeDetailsWithRating;
        public IEnumerable<vw_recipeUploadersNameWithRating> recipeUploadersNameWithRatingReadMore;
        public IEnumerable<vw_UsersUploadCounts> userUploadCounts;
        public vw_AllUserColWithRecipeID uploadersProfilePic;
        public IEnumerable<vw_AllRecipeDetailsWithFoodCategoryName> allRecipeWithFoodCategoryName;
        public IEnumerable<string> allergy;
        public IEnumerable<string> foodCategories;
        public IEnumerable<string> intolerance;
        public UserPremium userPremium;
        public vw_FoodSaleView foodSaleView;
        public IEnumerable<int> faveRecipes;
    }
}