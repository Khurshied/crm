using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    /// <summary>
    /// To hold total quantity sold in a month for chart use only.
    /// </summary>
    public class MonthVolume
    {
        public int Month { get; set; }
        public int Quantity { get; set; }
        public MonthVolume()
        {
        }
        public MonthVolume(int month, int quantity)
        {
            this.Month = month;
            this.Quantity = quantity;
        }
    }
}