using gatech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gatech.Controllers
{
    public class AccountController : Controller
    {
        private Context db = new Context();
        // GET: Account
        public ActionResult Index()
        {
            List<UserAccount> users = db.UserAccounts.ToList();
            return View(users);
        }

        // Action method to confirm the user account
        public ActionResult ConfirmAccount(string email,string token)
        {
            
            // Validate the token and confirm the user account
            if (ValidateToken(email,token))
            {
                // Account confirmed
                // Update user account status or perform any other necessary actions
                var user = db.UserAccounts.Where(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                user.IsConfirmed = 1;
                db.SaveChanges();
                // Redirect to a confirmation success page or display a success message
                return RedirectToAction("ConfirmationSuccess");
            }
            else
            {
                // Token validation failed
                // Redirect to an error page or display an error message
                return RedirectToAction("ConfirmationError");
            }
        }

        // Method to validate the registration token
        private bool ValidateToken(string email, string token)
        {
            bool isValid = false;
            // Implement your token validation logic here
            var user = db.UserAccounts.Where(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            // Compare the provided token with the one stored in the database
            if (user.RegToken.Equals(token))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            // Return true if the token is valid; otherwise, return false
            return isValid; 
        }

        public ActionResult AccessDenied()
        {
            return View();

        }

        public ActionResult ConfirmationSuccess()
        {
            ViewBag.Result = "Account Verified ";
            
            return View();
        }


    }
}