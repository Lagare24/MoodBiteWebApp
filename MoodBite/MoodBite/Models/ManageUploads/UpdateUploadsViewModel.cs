using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoodBite.Models.ManageUploads
{
    public class UpdateUploadsViewModel
    {
        public IEnumerable<RecipeIngredient> recipeIngredient;
        public Recipe recipe;
    }
}