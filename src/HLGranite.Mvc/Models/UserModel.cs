using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public partial class User
    {
        public static int ADMIN_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                UserType userType = db.UserTypes.Where(t => t.Type.ToLower().Equals("admin")).FirstOrDefault();
                if (userType == null)
                    return 0;
                else
                    return userType.Id;
            }
        }

        public static int STAFF_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                UserType userType = db.UserTypes.Where(t => t.Type.ToLower().Equals("staff")).FirstOrDefault();
                if (userType == null)
                    return 0;
                else
                    return userType.Id;
            }
        }

        public static int AGENT_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                UserType userType = db.UserTypes.Where(t => t.Type.ToLower().Equals("agent")).FirstOrDefault();
                if (userType == null)
                    return 0;
                else
                    return userType.Id;
            }
        }

        public static int CUSTOMER_TYPE_ID
        {
            get
            {
                hlgraniteEntities db = new hlgraniteEntities();
                UserType userType = db.UserTypes.Where(t => t.Type.ToLower().Equals("customer")).FirstOrDefault();
                if (userType == null)
                    return 0;
                else
                    return userType.Id;
            }
        }

        public bool IsAdmin()
        {
            return this.UserTypeId.Equals(ADMIN_TYPE_ID);
        }

        /// <summary>
        /// TODO: A unique username in system.
        /// </summary>
        public string UserName { get; set; }
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