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
    
    public partial class FoodSale
    {
        public int FoodSaleID { get; set; }
        public Nullable<int> UserRecipeID { get; set; }
        public Nullable<int> UserPremiumID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Address { get; set; }
    
        public virtual UserPremium UserPremium { get; set; }
        public virtual UserRecipe UserRecipe { get; set; }
    }
}