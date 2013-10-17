using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Muslim")]
        public Nullable<System.DateTime> Deathm { get; set; }
    }
}