using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public class MonthlyStock
    {
        public int Month { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MonthlyStock()
        {
        }
        public MonthlyStock(int month, string name, int quantity)
        {
            this.Month = month;
            this.Name = name;
            this.Quantity = quantity;
        }
    }
}