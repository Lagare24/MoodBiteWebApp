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
    
    public partial class Cart
    {
        public int CartID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> RecipeID { get; set; }
        public Nullable<int> FoodSaleID { get; set; }
        public Nullable<int> Qty { get; set; }
    
        public virtual FoodSale FoodSale { get; set; }
        public virtual FoodSale FoodSale1 { get; set; }
        public virtual FoodSale FoodSale2 { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual User User { get; set; }
    }
}
