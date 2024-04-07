using MoodBite.Models.RecipeViewModel;
using MoodBite.Models.UploadRecipeModel;
using System;
using System.Collections.Generic;
using System.IO;
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

            if (User.Identity.IsAuthenticated)
            {
                var recipeDetail = ReadMore(id);
                return View(recipeDetail);
            } else
            {
                return RedirectToAction("../Home/Index");
            }
        }

        public ActionResult UploadRecipe()
        {
            var model = new Recipe(); 
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadRecipe(Recipe recipe, string ingcount, string moodid, string[] ingredientName, int[] ingredientQty, string[] ingredientUnit)
        {
            var user = Session["User"] as User;
            var imageFile = Request.Files.Get("imageFile");

            var uploadRecipe = new Recipe();
            uploadRecipe.RecipeID = recipe.RecipeID;
            uploadRecipe.RecipeName = recipe.RecipeName;
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

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var recipeImage = new RecipeImage();
                recipeImage.RecipeID = newRecipeID;
                recipeImage.ImageName = recipe.RecipeName + " cover";

                using (var binaryReader = new BinaryReader(imageFile.InputStream))
                {
                    recipeImage.ImageURL = binaryReader.ReadBytes(imageFile.ContentLength);
                }

                _recipeImageRepo.Create(recipeImage);
            }

            var userRecipe = new UserRecipe();
            userRecipe.UserID = user.userID;
            userRecipe.RecipeID = newRecipeID;
            _userRecipeRepo.Create(userRecipe);

            var recipeIngredients = new RecipeIngredient();
            for (int i = 0; i < Convert.ToInt32(ingcount); i++)
            {
                recipeIngredients.RecipeID = newRecipeID;
                recipeIngredients.IngredientName = ingredientName[i];
                recipeIngredients.Unit = ingredientUnit[i];
                recipeIngredients.Quantity = ingredientQty[i];
                _recipeIngredientRepo.Create(recipeIngredients);
            }

            return View();
        }

    }
}