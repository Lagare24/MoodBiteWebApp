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
    
    public partial class vw_ForOrderPaymentInsertionView
    {
        public int OrderDetailID { get; set; }
        public Nullable<int> PO_ID { get; set; }
        public Nullable<int> FoodForSaleID { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string DeliveredTo { get; set; }
        public string DeliveredFrom { get; set; }
        public Nullable<System.DateTime> DateOrdered { get; set; }
        public Nullable<bool> IsPaid { get; set; }
    }
}