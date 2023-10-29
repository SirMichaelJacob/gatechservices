using gatech.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace gatech.Controllers
{
   [Authorize]
    public class DashboardController : Controller
    {
        private Context db = new Context();
        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                UserAccount currentUser = JsonConvert.DeserializeObject<UserAccount>(authTicket.UserData);

                //Locate the saved User Account on the db
                UserAccount userAcct = db.UserAccounts.Where(x => x.Email.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase)).First();
                //Assign it to current User Variable
                currentUser = userAcct;
                ViewBag.UserId = currentUser.Id;

                //// If the User is a Caregiver
                //if (UserHasRole(currentUser.Id))
                //{
                    //var careGiver = db.Caregivers.Where(x => x.Email.Equals(currentUser.Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();


                    if (currentUser.DateOfBirth == DateTime.MinValue || currentUser.ContactNumber == "" || currentUser.Address== "" || currentUser.Gender == "")
                    {
                        ViewBag.IncompleteProfile = true;
                    }

                //}
                
                return View(currentUser);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

           
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Dash(UserAccount user)
        {
            UserAccount userAcct = db.UserAccounts.Where(x => x.Email.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase)).First();

            //Without the following it wont work
            // Because the submitted model does not have an Id. It will assume you are creating a new account
            {
                userAcct.DateOfBirth = user.DateOfBirth;
                userAcct.ContactNumber = user.ContactNumber;
                userAcct.Address = user.Address;
                userAcct.Gender = user.Gender;
            }
            db.Entry(userAcct).State = EntityState.Modified;
            //db.UserAccounts.AddOrUpdate(userAcct);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public bool UserHasRole(int userId)
        {
            //var role = db.Roles.Where(x => x.Name.Equals("Caregiver", StringComparison.CurrentCultureIgnoreCase)).First();

            //Check if User has a Role
            var ur = db.UserRoles.Where(item => item.UserId==userId).FirstOrDefault();       
            //var ur = db.UserRoles.Where(x => x.UserId.Equals(userId)&& x.RoleId == role.Id);

            if (ur == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index","Login");
        }


        [HttpGet]
        public ActionResult Create()
        {
            var dbUsers = db.UserAccounts.ToList();
            var dbRoles = db.Roles.ToList();
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (UserAccount user in dbUsers)
            {
                users.Add(new SelectListItem { Text = user.Name +  " [" + user.Email + "]", Value = user.Id.ToString() });
            }

            List<SelectListItem> roles = new List<SelectListItem>();
            foreach (Role role in dbRoles)
            {
                roles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });
            }
            ViewBag.users = users;
            ViewBag.roles = roles;
            return View();
        }

        [HttpPost]
        //[AuthorizeUserAccess(myRole = "Admin")]
        public ActionResult Create(UserRole urole)
        {
            
            var xrole = db.UserRoles.Where(x => x.UserId.Equals(urole.UserId) && x.RoleId.Equals(urole.RoleId)).FirstOrDefault();
            if (xrole == null)
            {
                db.UserRoles.Add(urole);                
                db.SaveChanges();
                if(urole.RoleId==3)//Caregiver role
                {
                    UserAccount thisUser = db.UserAccounts.Single(x => x.Id == urole.UserId);
                    //Add User to Caregiver Table
                    Caregiver newCg = new Caregiver{Email = thisUser.Email };
                    db.Caregivers.Add(newCg);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "JoinUR");
            }
            else
            {
                ModelState.AddModelError("", "The selected User has this role already!");

                var dbUsers = db.UserAccounts.ToList();
                var dbRoles = db.Roles.ToList();
                List<SelectListItem> users = new List<SelectListItem>();
                foreach (UserAccount user in dbUsers)
                {
                    users.Add(new SelectListItem { Text = user.Name +" [" + user.Email + "]", Value = user.Id.ToString() });
                }

                List<SelectListItem> roles = new List<SelectListItem>();
                foreach (Role role in dbRoles)
                {
                    roles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });
                }
                ViewBag.users = users;
                ViewBag.roles = roles;

                return View();
            }

        }

        //[AuthorizeUserAccess(myRole = "Admin")]
        public ActionResult Delete(int urid, string uid, string rid)
        {
           
            var user = db.UserAccounts.Where(x => x.Id.ToString().Equals(uid)).FirstOrDefault();
            var role = db.Roles.Where(x => x.Id.ToString().Equals(rid)).FirstOrDefault();
            var userrole = db.UserRoles.Where(x => x.RoleId.ToString().Equals(rid) && x.UserId.ToString().Equals(uid)).FirstOrDefault();
            JoinUserRole userdata = new JoinUserRole();
            userdata.URId = userrole.Id;
            userdata.Name = user.Name;
            userdata.UserId = user.Id;
            userdata.Email = user.Email;
            userdata.RoleId = role.Id;
            userdata.RoleName = role.Name;

            return View(userdata);

        }

        //[AuthorizeUserAccess(myRole = "Admin")]
        [HttpPost]
        public ActionResult Delete(int urid)
        {
            UserRole userrole = db.UserRoles.Where(x => x.Id.ToString().Equals(urid.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            //UserRoles urole = db.userRoles.            

            //UserRoles userrole = db.userRoles.SingleOrDefault(x => x.ID.ToString() == urid.ToString());
            if (userrole != null)
            {
                db.UserRoles.Remove(userrole);
                db.SaveChanges();
                return RedirectToAction("Index", "JoinUR");
            }
            return View();
        }

    }
}