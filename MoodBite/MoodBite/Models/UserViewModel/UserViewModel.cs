using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoodBite.Models.UserViewModel
{
    public class UserViewModel
    {
        public User user;
        public UserRole userRole;
        public vw_RoleView userRoleView;
        public UserPremium userPremium;
        public Premium premiumType;
        public List<UserRecipe> userRecipe;
        public List<FoodSale> foodSale;
    }
}