using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoodBite.Models.UploadRecipeModel
{
    public class UploadRecipe
    {
        public Recipe recipeModel;
        public UserRecipe userRecipeModel;
        public RecipeIngredient recipeIngredientModel;
        public RecipeImage recipeImageModel;
        public string[] ingredientName;
        public int[] ingredientQty;
        public string[] ingredientUnit;

        public UploadRecipe()
        {
            recipeModel = new Recipe();
            userRecipeModel = new UserRecipe();
            recipeIngredientModel = new RecipeIngredient();
            recipeImageModel = new RecipeImage();
        }
    }
}