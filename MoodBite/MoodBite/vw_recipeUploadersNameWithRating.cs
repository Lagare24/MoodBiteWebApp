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
    
    public partial class vw_recipeUploadersNameWithRating
    {
        public int UserRecipeID { get; set; }
        public int userID { get; set; }
        public string UploadedBy { get; set; }
        public string RecipeName { get; set; }
        public Nullable<double> Rating { get; set; }
        public Nullable<int> TotalReview { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
    }
}