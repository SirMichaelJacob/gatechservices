using gatech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace gatech.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {

        public ActionResult Generate(int year, int month)
        {
            // Calculate the start and end dates for the specified month
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Generate the shift schedule for the specified month
            List<Shift> schedule = GenerateShiftSchedule(startDate, endDate);

            

            ViewBag.Title = String.Format("Shift for {0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),year);

           
            // Pass the generated schedule to the view
            return View(schedule);
        }

        private List<Shift> GenerateShiftSchedule(DateTime startDate, DateTime endDate)
        {
            List<Shift> schedule = new List<Shift>();

            // Generate shifts for each day in the specified month
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                schedule.Add(new Shift
                {
                    ShiftId = schedule.Count + 1,
                    ShiftName = "Morning Shift",
                    ShiftDate = date,
                    StartTime = TimeSpan.Parse("08:00"),
                    EndTime = TimeSpan.Parse("20:00")
                });

                schedule.Add(new Shift
                {
                    ShiftId = schedule.Count + 1,
                    ShiftName = "Evening Shift",
                    ShiftDate = date,
                    StartTime = TimeSpan.Parse("20:00"),
                    EndTime = TimeSpan.Parse("08:00")
                });
            }

            return schedule;
        }
    }

}