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
    
    public partial class vw_FilterAllergy
    {
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
        public string IngredientName { get; set; }
        public Nullable<double> Quantity { get; set; }
        public string Unit { get; set; }
        public string AllergyName { get; set; }
    }
}
