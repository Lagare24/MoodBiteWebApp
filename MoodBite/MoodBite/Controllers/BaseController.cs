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

        public BaseController()
        {
            _db = new MoodBiteEntities();
            _moodRepo = new BaseRepository<Mood>();
            _userRepo = new BaseRepository<MoodBite.User>();
            _recipeRepo = new BaseRepository<Recipe>();
            _recipeIngredientsRepo = new BaseRepository<vw_IngredientsOfRecipe>();
        }
        public bool IsUserAuthenticated()
        {
            if (User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }
    }
}