using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gatech.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace gatech.Controllers
{
    public class joinURController : Controller
    {
        // GET: joinUR

        [Route("view-user-roles")]
        public ActionResult Index()
        {
            List<JoinUserRole> AllUsersRoles = new List<JoinUserRole>();
            string conString = ConfigurationManager.ConnectionStrings["Context"].ConnectionString; //Connection String
            SqlConnection conn = new SqlConnection(conString); //Sql Connection Object
            string query = "Select UserRole.Id as URId, UserAccount.Id as UserId,UserAccount.Name as Name,Email,Role.Id as RoleId,Role.Name as RoleName from UserAccount inner join UserRole on UserAccount.Id = UserRole.UserId inner join Role on Role.Id = UserRole.RoleId order by UserAccount.Id ASC";
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    JoinUserRole UserData = new JoinUserRole();
                    //
                    UserData.URId = int.Parse(reader[0].ToString());
                    UserData.UserId = int.Parse(reader[1].ToString());
                    UserData.Name = reader[2].ToString();
                   
                    UserData.Email = reader[3].ToString();
                    UserData.RoleId= int.Parse(reader[4].ToString());
                    UserData.RoleName = reader[5].ToString();
                    //
                    AllUsersRoles.Add(UserData);
                }
            }
            conn.Close();

            return View(AllUsersRoles);
        }
    }
}