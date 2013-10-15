using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public partial class Nisan
    {
        public override string ToString()
        {
            if (String.IsNullOrEmpty(this.Rumi))
                return "(tiada nama)";
            else
                return this.Rumi;
        }
    }
}