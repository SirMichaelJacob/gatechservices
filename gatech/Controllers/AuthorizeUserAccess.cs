using gatech.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public class AuthorizeUserAccess : AuthorizeAttribute
    {
        public string myRole { get; set; } //The specified Access Role


        protected override bool AuthorizeCore(HttpContextBase curContext)
        {
            string currentUserID = HttpContext.Current.User.Identity.Name; //Currrent LoggedIn User ID
            var isAuthorized = base.AuthorizeCore(curContext);
            if (!isAuthorized)
            {
                return false;
            }



            bool UserHasRole = false; //Variable to return the result of DB check



            //Get User Roles of the current user from db
            string conString = System.Configuration.ConfigurationManager.ConnectionStrings["Context"].ConnectionString;
            SqlConnection conn = new SqlConnection(conString);

            string query = "Select Role.Name from Role inner join UserRole on RoleId = Role.Id where UserId = '" + currentUserID + "'";

            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            List<string> allRoles = new List<string>(); //string list of UserRoles

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    allRoles.Add(reader[0].ToString());
                }
            }
            conn.Close();
            ///


            if (myRole != null) //When role is specified
            {
                var arr = myRole.Split(',');

                foreach (string role in allRoles)
                {

                    if (arr.Contains(role))
                    {
                        UserHasRole = true;
                        break;
                    }
                }
                return UserHasRole;
            }
            else
            {


                return true;
            }


        }


        //public bool IsInRole(string userId)
        //{
        //    MyDBContext db = new MyDBContext();
        //    var user = db.userRoles.Where(x => x.UserID.ToString().Equals(userId));
        //    if (user == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{
        //    string currentUserID = HttpContext.Current.User.Identity.Name; //Currrent LoggedIn User ID
        //    if (!filterContext.HttpContext.Request.IsAuthenticated) base.OnAuthorization(filterContext);

        //    if (string.IsNullOrEmpty(Roles)) return;
        //    if (currentUserID == null) return;

        //    if (!IsInRole(currentUserID)) filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
        //}

    }
}