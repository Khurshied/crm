using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLGranite.Mvc.Models
{
    public partial class User
    {
        public static short ADMIN_TYPE_ID
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

        public static short STAFF_TYPE_ID
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

        public static short AGENT_TYPE_ID
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

        public static short CUSTOMER_TYPE_ID
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
        /// <summary>
        /// True if is admin otherwise false.
        /// </summary>
        public bool IsAdmin
        {
            get { return this.UserTypeId == ADMIN_TYPE_ID; }
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
            get {
                if (this == null)
                    return string.Empty;
                else
                    return this.FirstName + " " + this.LastName;
            }
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