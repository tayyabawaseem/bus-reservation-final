using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Employee")]
    public class EmployeeDashboardController : Controller
    {

        private readonly BusReservationSystemContext db;

        public EmployeeDashboardController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1).Date;

            var dashboardViewModel = new DashboardViewModel
            {
                BusesToday = db.Buses
    .Where(b => b.DepartureTime.Date == today || b.ArrivalTime.Date == today)
    .Include(b => b.BusType)                // Include BusType
                .Include(b => b.Route)                   // Include Route
                .ThenInclude(r => r.StartingPlaceNavigation) // Include StartingPlace navigation
                .Include(r => r.Route.DestinationPlaceNavigation) // Include DestinationPlace navigation
                .OrderByDescending(l => l.BusId).ToList(),
                BusesTomorrow = db.Buses
    .Where(b => b.DepartureTime.Date == tomorrow || b.ArrivalTime.Date == tomorrow)
    .Include(b => b.BusType)                // Include BusType
                .Include(b => b.Route)                   // Include Route
                .ThenInclude(r => r.StartingPlaceNavigation) // Include StartingPlace navigation
                .Include(r => r.Route.DestinationPlaceNavigation) // Include DestinationPlace navigation
                .OrderByDescending(l => l.BusId).ToList()
            };

            return View(dashboardViewModel);
        }

        public IActionResult Profile()
        {
            var id = int.Parse(User.FindFirst("EmployeeID")?.Value);
            var employee = db.Employees
            .Include(e => e.Branch)
                             .FirstOrDefault(m => m.Id == id);


            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(Employee employeepass)
        {
            var password = employeepass.Password;
            // Get the employee ID from the current user's claims
            var employeeId = int.Parse(User.FindFirst("EmployeeID")?.Value);

            // Fetch the employee from the database
            var employee = await db.Employees.FindAsync(employeeId);

            var hasher = new PasswordHasher<string>();
            string hashPassword = hasher.HashPassword(employee.Email, employeepass.Password);
            employee.Password = hashPassword;

         

            // Save changes to the database
            db.Employees.Update(employee);
            await db.SaveChangesAsync();

            string message = $"Hi {employee.Name},\n\nYour Password has been updated successfully!\n\n" +
                     $"Email: {employee.Email}\nPassword: {password}\n\n" +
                     "We suggest you to change your password.";

            SendEmail(employee.Email, message, "Update Password");
            // Optionally add a success message
            TempData["Message"] = "Password changed successfully.";

            return RedirectToAction("Profile"); // Redirect to profile or another appropriate page
        }

        public bool SendEmail(string email, string message, string subject)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("abdullah.siddiqui13122002@gmail.com", "sawe sczj ibte rnvj");

            MailMessage msg = new MailMessage("abdullah.siddiqui13122002@gmail.com", email);
            msg.Subject = subject;
            msg.Body = message;

            client.Send(msg);
            ViewBag.message = "Message sent successfully.";
            return true;
        }

    }
}
