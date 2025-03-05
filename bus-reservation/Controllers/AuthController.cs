using bus_reservation.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bus_reservation.Controllers
{
    public class AuthController : Controller
    {

        private readonly BusReservationSystemContext db;

        public AuthController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Login()
        {
            // Prevent caching of the login page
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Redirect if the user is already authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                // Redirect based on role
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("Employee"))
                {
                    return RedirectToAction("Index", "EmployeeDashboard");
                }
            }
            else
            {
                return View();
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.msg = "Email and password cannot be empty.";
                return View();
            }

            var checkUser = db.Employees.FirstOrDefault(a => a.Email == Email);
            var checkadmin = db.Admins.FirstOrDefault(a => a.Email == Email);

            if (checkUser != null)
            {
                var hasher = new PasswordHasher<string>();
                var verifyPass = hasher.VerifyHashedPassword(Email, checkUser.Password, Password);

                if (verifyPass == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
        {
                        new Claim("EmployeeID", checkUser.Id.ToString()),
                        new Claim("BranchID", checkUser.BranchId.ToString()),
                        new Claim("EmployeeEmail", checkUser.Email),
                        new Claim(ClaimTypes.Role, "Employee"),
                        new Claim("EmployeeName", checkUser.Name)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                    return RedirectToAction("Index", "EmployeeDashboard"); // Redirect to the appropriate controller/action
                }
                else
                {
                    ViewBag.msg = "Invalid Credentials";
                    return View();
                }
            }
            else if (checkadmin != null)
            {
                //var hasher = new PasswordHasher<string>();
                //var verifyPass = hasher.VerifyHashedPassword(Email, checkadmin.Password, Password);

                if (checkadmin.Password == Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("AdminID", checkadmin.Id.ToString()),
                        new Claim("AdminEmail", checkadmin.Email),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetInt32("AdminID", checkadmin.Id);
                    HttpContext.Session.SetString("AdminEmail", checkadmin.Email);

                    return RedirectToAction("Index", "Admin"); // Redirect to the appropriate controller/action
                }
                else
                {
                    ViewBag.msg = "Invalid Credentials";
                    return View();
                }
            }
            else
            {
                ViewBag.msg = "Invalid User";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(".AspNetCore.Cookies");
            return RedirectToAction("Login", "Auth");
        }


    }
}
