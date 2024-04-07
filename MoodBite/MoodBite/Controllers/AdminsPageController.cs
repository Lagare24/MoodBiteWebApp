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

        public ActionResult EditRecipe(int id)
        {
            var ingredientCount = _db.Recipe.Where(model => model.RecipeID == id).Select(model => model.IngredientsCount).FirstOrDefault();
            var recipe = _db.RecipeIngredient.Where(model => model.RecipeID == id).ToList();

            var ingredientName = new List<string>();
            var ingredientQty = new List<int>();
            var ingredientUnit = new List<string>();

            foreach (var item in recipe)
            {
                ingredientName.Add(item.IngredientName);
                ingredientQty.Add(Convert.ToInt32(item.Quantity));
                ingredientUnit.Add(item.Unit);
            }

            return View(_recipeRepo.Get(id));
        }

        [HttpPost]
        public ActionResult EditRecipe(Recipe recipe)
        {
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