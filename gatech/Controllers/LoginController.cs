using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GoogleAuthentication.Services;
using gatech.Models;
using Newtonsoft.Json;
using System.Web.Security;
using System.Security.Claims;
using System.Net.Mail;

namespace gatech.Controllers
{
    public class LoginController : Controller
    {
        private Context db = new Context();
        // GET: Login
        public ActionResult Index()
        {
            var clientId= "334252712672-hcfo9502i454a7v5fbjjb54v7dsj1huc.apps.googleusercontent.com";
            var url= "https://localhost:44391/Login/GoogleLoginCallback";
            var response = GoogleAuth.GetAuthUrl(clientId,url);
            ViewBag.response = response;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Account");
            }
            return View();
        }

        public async Task<ActionResult> GoogleLoginCallback(string code)
        {
            var clientId = "334252712672-hcfo9502i454a7v5fbjjb54v7dsj1huc.apps.googleusercontent.com";
            var url = "https://localhost:44391/Login/GoogleLoginCallback"; 
            var clientSecret = "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";
            var token = await GoogleAuth.GetAuthAccessToken(code,clientId,clientSecret,url);

            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());

            GoogleProfile CustomUser = JsonConvert.DeserializeObject<GoogleProfile>(userProfile);

            UserAccount newUser = db.UserAccounts.SingleOrDefault(x => x.Email.ToLower() == CustomUser.Email.ToLower());

            //var account = db.UserAccounts.Where(x => x.Email.Equals(CustomUser.Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var RegToken = GenerateRegistrationToken();

            if (newUser == null)
            {
                newUser = new UserAccount() {
                    Name = CustomUser.Name,
                    Email = CustomUser.Email,
                    DateOfBirth = DateTime.MinValue,
                    RegToken = RegToken,
                    IsConfirmed = 0
                };

                db.UserAccounts.Add(newUser);
                db.SaveChanges();
                var account = db.UserAccounts.Where(x => x.Email.Equals(CustomUser.Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //Send Confirmation Email
                SendConfirmationEmail(account.Email, RegToken);

                //return RedirectToAction("RegistrationSuccess","Account");
                return View("RegistrationSuccess", account);
                //Authenticate User
                //AuthenticateUser(account);
                
            }
            else
            {
                //The User exists
                //Check if the User Email is Confirmed
                if (newUser.IsConfirmed == 1)
                {
                    //Authenticate User
                    AuthenticateUser(newUser);
                    //To User Dashboard
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    var newToken = GenerateRegistrationToken();
                    newUser.RegToken = newToken;
                    db.SaveChanges();
                    SendConfirmationEmail(newUser.Email, newToken);

                    return View("RegistrationSuccess", newUser);
                }

            }
            
                
        }

        // Method to generate a registration token
        private string GenerateRegistrationToken()
        {
            // Generate a unique identifier using GUID
            string uniqueId = Guid.NewGuid().ToString();

            // Generate a random password using Membership.GeneratePassword
            string password = Membership.GeneratePassword(10, 3);

            // Concatenate the unique identifier and password to create the token
            string token = uniqueId + password;

            return token;
        }

        // Method to send the confirmation email
        private void SendConfirmationEmail(string userEmail, string token)
        {
            // Configure the SMTP settings for your email provider
            //SmtpClient smtpClient = new SmtpClient("smtp.yourprovider.com", 587);
            SmtpClient smtpClient = new SmtpClient("mail.ccpgroup.com.ng", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@ccpgroup.com.ng", "Mykel101#");

            // Create the email message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("info@ccpgroup.com.ng"); // Replace with your email address
            mailMessage.To.Add(userEmail);
            mailMessage.Subject = "GA Tech Account Confirmation";
            mailMessage.Body = "Dear User, please click the link to confirm your account: " +
                               "<a href='" + Url.Action("ConfirmAccount", "Account", new {email=userEmail, token=token }, Request.Url.Scheme) + "'>Confirm Account</a>";

            // Set the email body as HTML
            mailMessage.IsBodyHtml = true;

            // Send the email
            smtpClient.Send(mailMessage);
        }
        //Registration Success
        public ActionResult RegistrationSuccess(UserAccount newUser)
        {

            return View(newUser);
        }


        //User Authentication
        public void AuthenticateUser(UserAccount account)
        {
            FormsAuthentication.SetAuthCookie(account.Id.ToString(), true);

            var serializedUser = Newtonsoft.Json.JsonConvert.SerializeObject(account);
            var ticket = new FormsAuthenticationTicket(1, account.Email, DateTime.Now, DateTime.Now.AddHours(3), true, serializedUser);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var isSsl = Request.IsSecureConnection; // if we are running in SSL mode then make the cookie secure only

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true, // always set this to true!
                Secure = isSsl,
            };
            
            Response.Cookies.Set(cookie);


            var currentUserID = HttpContext.User.Identity.Name;
            Session["UserID"] = account.Id.ToString();

            //Check and Save Roles
            var LoggedUserRoles = getUserRoles(account.Id.ToString());

            foreach (var role in LoggedUserRoles)
            {

                System.Web.HttpContext.Current.Session[role] = role;
            }

        }
        public List<string> getUserRoles(string userid)
        {
            List<string> roleIds = new List<string>();
            var userrole = db.UserRoles.Where(x => x.UserId.ToString().Equals(userid));
            foreach (var item in userrole)
            {
                roleIds.Add(item.RoleId.ToString());
            }

            List<string> roles = new List<string>();
            foreach (var id in roleIds)
            {
                var role = db.Roles.Single(x => x.Id.ToString().Equals(id));
                roles.Add(role.Name);
            }
            return roles;

        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }

    
}