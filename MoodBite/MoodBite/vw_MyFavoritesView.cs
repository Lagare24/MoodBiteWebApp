//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MoodBite
{
    using System;
    using System.Collections.Generic;
    
    public partial class vw_MyFavoritesView
    {
        public int UsersFaveRecipeID { get; set; }
        public int userID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public Nullable<bool> EmailConfirmed { get; set; }
        public string EmailConfirmationToken { get; set; }
        public string ProfilePicturePath { get; set; }
        public int RecipeID { get; set; }
        public Nullable<int> MoodID { get; set; }
        public Nullable<int> FoodCategoryID { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public Nullable<int> IngredientsCount { get; set; }
        public string CookingInstruction { get; set; }
        public Nullable<System.TimeSpan> PreparationTime { get; set; }
        public Nullable<System.TimeSpan> CookingDuration { get; set; }
        public Nullable<System.DateTime> DateUploaded { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<System.DateTime> DateApproved { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string FoodCategoryName { get; set; }
    }
}