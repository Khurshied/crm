using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HLGranite.Mvc.Models
{
    public class AuthorizeAdmin : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAdmin = false;
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                // the user is either not authenticated or
                // not in roles => no need to continue any further
                return false;
            }

            // get the currently logged on user
            var username = httpContext.User.Identity.Name;

            // get the id of the article that he is trying to manipulate
            // from the route data (this assumes that the id is passed as a route
            // data parameter: /foo/edit/123). If this is not the case and you 
            // are using query string parameters you could fetch the id using the Request
            //var id = httpContext.Request.RequestContext.RouteData.Values["id"] as string;

            // Now that we have the current user and the id of the article he
            // is trying to manipualte all that's left is go ahead and look in 
            // our database to see if this user is the owner of the article
            HLGranite.Mvc.Models.hlgraniteEntities db = new HLGranite.Mvc.Models.hlgraniteEntities();
            HLGranite.Mvc.Models.User user = db.Users.Where(u => u.UserName.Equals(username)).FirstOrDefault();
            if (user != null) isAdmin = user.IsAdmin;

            return isAdmin;
        }
    }
}