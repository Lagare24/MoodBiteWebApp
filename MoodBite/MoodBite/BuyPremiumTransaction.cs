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
    
    public partial class BuyPremiumTransaction
    {
        public int transactionID { get; set; }
        public string customerEmail { get; set; }
        public Nullable<int> premiumTypeID { get; set; }
        public Nullable<decimal> amoountPaid { get; set; }
        public string phoneNumber { get; set; }
    
        public virtual Premium Premium { get; set; }
    }
}