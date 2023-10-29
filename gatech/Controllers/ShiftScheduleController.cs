using gatech.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace gatech.Controllers
{
    public class ShiftScheduleController : Controller
    {
        private Context db = new Context();
        // GET: ShiftSchedule
        public ActionResult Index()
        {
            return View();
        }

        //Edit Shift
        public ActionResult Edit(int ClientId, int CaregiverId, string ShiftName,DateTime ShiftDate,TimeSpan StartTime,TimeSpan EndTime)
        {
            Shift shift = new Shift {
                ShiftName = ShiftName,
                ShiftDate = ShiftDate,
                StartTime = StartTime,
                EndTime = EndTime,
                ClientId = ClientId,
                CaregiverId = CaregiverId
            };
            List<SelectListItem> users = new List<SelectListItem>();
            // Retrieve Users from the database
            var dbUsers = db.UserAccounts.ToList();
            foreach (UserAccount user in dbUsers)
            {
                users.Add(new SelectListItem { Text = user.Name + " [" + user.Email + "]", Value = user.Id.ToString() });
            }

            ViewBag.users = users;

            return View(shift);

        }


        [ActionName("Edit")]
        //[Route("view-shifts/edit/{id}")]
        [HttpPost]
        public ActionResult Edit(Shift selShift)
        {
            if (!ModelState.IsValid)
            {
                return View(selShift);
            }
            /*2 ways of updating DB record*/
            db.Shifts.AddOrUpdate(selShift); //Must add 'using System.Data.Entity.Migrations';

            //db.Entry(selShift).State = EntityState.Modified;
            //
            db.SaveChanges();

            return Content(selShift.CaregiverId.ToString());

            //return RedirectToAction("Details", new { id = selShift.ShiftId });
        }

        //Show User Shifts
        public ActionResult MyRota()
        {
            UserAccount user = db.UserAccounts.Where(x => x.Email.Equals(HttpContext.User.Identity.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            int userId = user.Id;
            ViewBag.UserId = userId;
            return View();
        }

        public ActionResult ViewRota(int year,int month, int userId)
        {
            UserAccount user = db.UserAccounts.Where(x => x.Email.Equals(HttpContext.User.Identity.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            userId = user.Id;
            var rota = getUserRota(year, month, userId);
            ViewBag.Year = year;
            ViewBag.Month = month;
            return View(rota);
        }

        // GET: ShiftSchedule/Generate/{year}/{month}
        public ActionResult Generate(int year, int month)
        {
            // Calculate the start and end dates for the specified month
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            // Retrieve clients from the database
            List<Client> clients = db.Clients.ToList();
            // Retrieve Users from the database
            var dbUsers = db.UserAccounts.ToList();
            
            List<SelectListItem> users = new List<SelectListItem>();
            foreach (UserAccount user in dbUsers)
            {
                users.Add(new SelectListItem { Text = user.Name + " [" + user.Email + "]", Value = user.Id.ToString() });
            }


            // Generate shift schedules for each client
            List<Shift> shiftSchedules = GenerateShiftSchedules(startDate,endDate, clients);

            ViewBag.Clients = clients;
            ViewBag.users = users;
            ViewBag.Year = year;
            ViewBag.Month = month;
            return View(shiftSchedules);
        }


        // Generate shift schedules for each client
        private List<Shift> GenerateShiftSchedules(DateTime startDate, DateTime endDate, List<Client> clients)
        {
            List<Shift> schedule = new List<Shift>();
            List<Shift> scheduleInDB = db.Shifts.ToList();
            bool foundMorning = false;
            bool foundEvening = false;

            foreach (Client client in clients)
            {
                // Generate shifts for each day in the specified month
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    Shift newMShift = new Shift();
                    Shift newEShift = new Shift();
                    foundMorning = false;
                    foundEvening = false;
                    //Shift newShift = new Shift();
                    foreach (Shift shift in scheduleInDB)
                    {
                        
                        if (shift.ShiftDate.ToShortDateString() == date.ToShortDateString() &&  shift.CaregiverId != 0)//shift exists
                        {
                            if (shift.ShiftName == "Morning Shift") {
                                foundMorning = true;
                                newMShift = shift;
                            }

                            if( shift.ShiftName == "Evening Shift")
                            {
                                foundEvening = true;
                                newEShift = shift;
                            }
                            
                        }

                    }
                    //Generate New Schedule
                    if (foundMorning == false)
                    {
                        newMShift = new Shift()
                        {
                            ShiftId = schedule.Count + 1,
                            ShiftName = "Morning Shift",
                            ShiftDate = date,
                            StartTime = TimeSpan.Parse("08:00"),
                            EndTime = TimeSpan.Parse("20:00"),
                            ClientId = client.ClientId,
                            CaregiverId = 0
                        };
                    }

                    if (foundEvening == false)
                    {
                        newEShift = new Shift()
                        {
                            ShiftId = schedule.Count + 1,
                            ShiftName = "Evening Shift",
                            ShiftDate = date,
                            StartTime = TimeSpan.Parse("20:00"),
                            EndTime = TimeSpan.Parse("08:00"),
                            ClientId = client.ClientId,
                            CaregiverId = 0
                        };
                    }

                    if (!schedule.Contains(newMShift))
                    {
                        schedule.Add(newMShift);
                    }
                    if (!schedule.Contains(newEShift))
                    {
                        schedule.Add(newEShift);
                    }
                   
                    //End of date loop
                }
                //End of client loop
            }

            return schedule;
        }

        public string getUser(int? userid)
        {
            if (userid != null)
            {
                var user = db.UserAccounts.Where(x => x.Id.ToString() == userid.ToString()).FirstOrDefault();
                var result = "";
                if (user == null)//User not found (Maybe deleted)
                {

                    result = String.Format("<text class='text-danger'>([{0}] Not found or Deleted)</text>", userid.ToString());
                    if (user == null && userid == 0)
                    {
                        result = "--Vacant--";
                    }
                    return result;
                }
                else if (userid > 0)
                {
                    return String.Format("{0} {1}.", user.Name, user.Email);
                }
                else
                {
                    return "--Vacant--";
                }

            }
            else
            {
                return "--Vacant--";
            }


        }

        public string getClient(int? clientId)
        {
            if (clientId != null)
            {
                var client = db.Clients.Where(x => x.ClientId.ToString() == clientId.ToString()).FirstOrDefault();
                var result = "";
                if (client == null)//User not found (Maybe deleted)
                {

                    result = String.Format("<text class='text-danger'>([{0}] Not found or Deleted)</text>", clientId.ToString());
                    if (client == null && clientId == 0)
                    {
                        result = "--Vacant--";
                    }
                    return result;
                }
                else if (clientId > 0)
                {
                    return String.Format("{0} {1} - ({2}).", client.FirstName, client.LastName[0],client.Email);
                }
                else
                {
                    return "--Vacant--";
                }

            }
            else
            {
                return "--Vacant--";
            }


        }

        /**Get Client Name**/
        public string getClientName(int? clientId)
        {
            if (clientId != null)
            {
                var client = db.Clients.Where(x => x.ClientId.ToString() == clientId.ToString()).FirstOrDefault();
                var result = "";
                if (client == null)//User not found (Maybe deleted)
                {

                    result = String.Format("<text class='text-danger'>([{0}] Not found or Deleted)</text>", clientId.ToString());
                    if (client == null && clientId == 0)
                    {
                        result = "--Vacant--";
                    }
                    return result;
                }
                else if (clientId > 0)
                {
                    return String.Format("{0} {1}.", client.FirstName, client.LastName);
                }
                else
                {
                    return "--Vacant--";
                }

            }
            else
            {
                return "--Vacant--";
            }


        }

        //Get User Rota

        public List<Shift> getUserRota(int year, int month,int userId)
        {
            List<Shift> myMonthRota = new List<Shift>();
            List<Shift> allShifts = db.Shifts.ToList(); //Retrieve shifts fro database
            foreach(Shift shift in allShifts)
            {
                if(shift.CaregiverId==userId && shift.ShiftDate.Month==month && shift.ShiftDate.Year == year)
                {
                    myMonthRota.Add(shift);
                }
            }
            return myMonthRota;
        }
    }
}