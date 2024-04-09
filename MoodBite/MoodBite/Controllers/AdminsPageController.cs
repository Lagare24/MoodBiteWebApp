using MoodBite.Models.ManageUploads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsPageController : BaseController
    {
        //Manage Users
        public ActionResult ManageUsers()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_db.vw_ManageUsers.ToList());
            }
            return RedirectToAction("./Account/Login");
        }
        public ActionResult ManageUploads()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_db.vw_ManageUploads.ToList());
            }
            return RedirectToAction("./Account/Login");
        }

        public ActionResult ApproveRecipe(int id)
        {
            var adminInCharge = Session["User"] as User;
            var recipe = _db.Recipe.Where(model => model.RecipeID == id).FirstOrDefault();

            recipe.IsApproved = true;
            recipe.ApprovedBy = adminInCharge.userID;
            recipe.DateApproved = DateTime.Today;

            _recipeRepo.Update(id, recipe);

            return RedirectToAction("ManageUploads");
        }

        public ActionResult RejectRecipe(int id)
        {
            var adminInCharge = Session["User"] as User;
            var recipe = _db.Recipe.Where(model => model.RecipeID == id).FirstOrDefault();

            recipe.IsApproved = false;
            recipe.ApprovedBy = adminInCharge.userID;
            recipe.DateApproved = DateTime.Today;

            _recipeRepo.Update(id, recipe);

            return RedirectToAction("ManageUploads");
        }

        public ActionResult EditRecipe(int id)
        {
            if(User.Identity.IsAuthenticated)
            {
                var recipe = _recipeRepo.Get(id);
                var recipeIngredients = _db.RecipeIngredient.Where(m => m.RecipeID == id).ToList();

                UpdateUploadsViewModel model = new UpdateUploadsViewModel();
                model.recipe = recipe;
                model.recipeIngredient = recipeIngredients;


                return View(model);
            }
            return RedirectToAction("ManageUploads");
        }

        [HttpPost]
        public ActionResult EditRecipe(UpdateUploadsViewModel postModel, string ingcount, string moodid, string[] ingredientName, int[] ingredientQty, string[] ingredientUnit)
        {
            var recipe = new Recipe();
            recipe.RecipeID = postModel.recipe.RecipeID;
            recipe.RecipeName = postModel.recipe.RecipeName;
            recipe.RecipeDescription = postModel.recipe.RecipeDescription;
            recipe.PreparationTime = TimeSpan.Parse(postModel.recipe.PreparationTime.ToString());
            recipe.CookingDuration = TimeSpan.Parse(postModel.recipe.CookingDuration.ToString());
            recipe.DateUploaded = DateTime.Today;
            recipe.CookingInstruction = postModel.recipe.CookingInstruction;
            recipe.IngredientsCount = Convert.ToInt32(ingcount);
            recipe.IsApproved = false;
            recipe.MoodID = Convert.ToInt32(moodid);
            _recipeRepo.Create(recipe);

            int newRecipeID = postModel.recipe.RecipeID;

            var userRecipe = new UserRecipe();
            var recipeIngredients = new RecipeIngredient();
            var recipeImage = new RecipeImage();



            return RedirectToAction("ManageUploads");
        }

        public ActionResult ManageSubscriptions()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(_db.vw_ManageSubscriptions.ToList());
            }
            return RedirectToAction("./Account/Login");
        }
    }
}