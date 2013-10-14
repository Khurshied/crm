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
        /// <summary>
        /// For display at login page purpose.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Override toString value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this != null)
                return DisplayName;
            else
                return string.Empty;
        }
    }
}