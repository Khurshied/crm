using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public partial class User
    {
        /// <summary>
        /// Return full name for a user.
        /// </summary>
        public string DisplayName
        {
            get { return this.FirstName + " " + this.LastName; }
        }
    }
}