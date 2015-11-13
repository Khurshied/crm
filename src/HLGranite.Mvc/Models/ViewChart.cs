using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    /// <summary>
    /// A model to hold chart image.
    /// </summary>
    public class ViewChart
    {
        public byte[] Raw { get; set; }
        public ViewChart()
        {
        }
        public ViewChart(byte[] bytes)
        {
            this.Raw = bytes;
        }
    }
}