//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiniMerce.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class transaction
    {
        public int ID { get; set; }
        public string payment_method { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<double> total { get; set; }
        public string status { get; set; }
        public Nullable<int> order_id { get; set; }
        public string shipping_method { get; set; }
    }
}
