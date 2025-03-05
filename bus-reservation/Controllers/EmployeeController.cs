using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly BusReservationSystemContext db;

        public EmployeeController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var Employee = db.Employees.OrderByDescending(l => l.Id).ToList(); // Ensure this is not null
            return View(Employee);
        }

        public IActionResult Details(int? id)
        {
            

            var employee = db.Employees
                             .Include(e => e.Branch) // Assuming you have a navigation property Branch in Employee
                             .FirstOrDefault(m => m.Id == id);

            if (employee == null)
            {
                return NotFound(); // Handle the case when no employee is found
            }

            return View(employee);
        }



        public IActionResult Create()
        {
            var branches = db.Branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.BranchCode} - {b.Address}"
            }).ToList();

            ViewBag.Branches = branches;

           

            return View();
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee employee, IFormFile file)
        {

            if (db.Employees.Any(e => e.Email == employee.Email))
            {
                ViewBag.message = "Email already exists.";
                var branches = db.Branches.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"{b.BranchCode} - {b.Address}"
                }).ToList();

                ViewBag.Branches = branches;
                return View(employee);
            }
            
            else
            {
                string imagename = DateTime.Now.ToString("yymmddhhmmss");
                imagename += "-" + Path.GetFileName(file.FileName);

                var imagepath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/admin/images/employee_images");
                var imageValue = Path.Combine(imagepath, imagename);

                using (var stream = new FileStream(imageValue, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var dbimage = Path.Combine("/admin/images/employee_images", imagename);

                employee.EmployeeImage = dbimage;

                var Password = employee.Password;

                var hasher = new PasswordHasher<string>();
                string hashPassword = hasher.HashPassword(employee.Email, employee.Password);
                employee.Password = hashPassword;

                db.Employees.Add(employee);
                db.SaveChanges();




                string message = $"Welcome {employee.Name},\n\nYour account has been created successfully!\n\n" +
                     $"Email: {employee.Email}\nPassword: {Password}\n\n" +
                     "We suggest you to change your password.";

                SendEmail(employee.Email, message, "Welcome to the Team");

  
                TempData["SuccessCreateMessage"] = "Employee Added Successfully!";

                return RedirectToAction("Index");
            }

           
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || db.Employees == null)
            {
                return NotFound();
            }

            var employee = db.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            var branches = db.Branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.BranchCode} - {b.Address}"
            }).ToList();

            ViewBag.Branches = branches;



            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee employee, IFormFile file, string oldImage, string password, string oldPassword, string oldEmail)
        {

            if (db.Employees.Any(e => e.Email == employee.Email && e.Id != employee.Id))
            {
                ViewBag.message = "Email already exists.";
                var branches = db.Branches.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = $"{b.BranchCode} - {b.Address}"
                }).ToList();

                ViewBag.Branches = branches;
                return View(employee);
            }

            else
            {
                if (file != null && file.Length > 0)
                {
                    string imagename = DateTime.Now.ToString("yymmddhhmmss");
                    imagename += "-" + Path.GetFileName(file.FileName);

                    var imagepath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/admin/images/employee_images");
                    var imageValue = Path.Combine(imagepath, imagename);

                    using (var stream = new FileStream(imageValue, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var dbimage = Path.Combine("/admin/images/employee_images", imagename);

                    employee.EmployeeImage = dbimage;
                }
                else
                {
                    employee.EmployeeImage = oldImage;
                }

                if (password != null && password.Length > 0) {
                    var hasher = new PasswordHasher<string>();
                    string hashPassword = hasher.HashPassword(employee.Email, password);
                    employee.Password = hashPassword;
                }
                else
                {
                    employee.Password = oldPassword;
                }

                db.Employees.Update(employee);
                db.SaveChanges();

                if (password != null && password.Length > 0 || employee.Email != oldEmail)
                {
                    string message = $"Hi {employee.Name},\n\nYour account has been updated successfully!\n\n" +
                    $"Email: {employee.Email}\nPassword: {password}\n\n";

                    SendEmail(employee.Email, message, "Your Updated Information");
                }
                TempData["SuccessEditMessage"] = "Employee Edited Successfully!";

                return RedirectToAction("Index");
            }


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


        public IActionResult Delete(int? id)
        {

            var employee = db.Employees.Find(id);



            var relatedBookings = db.Bookings
                                      .Where(e => e.EmployeeId == id)
                                      .ToList();

 


            if (relatedBookings.Any())
            {
                db.Bookings.RemoveRange(relatedBookings);
            }




            db.Employees.Remove(employee);
            db.SaveChanges();

            TempData["SuccessDeleteMessage"] = "Employee and all related records deleted successfully!";
            return RedirectToAction("Index");
        }

    }
}
