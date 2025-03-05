using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Admin")]
    public class BranchController : Controller
    {
        private readonly BusReservationSystemContext db;

        public BranchController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var Branches = db.Branches.OrderByDescending(l => l.Id).ToList();
            return View(Branches);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Branch branch)
        {

            var duplicateZip = db.Branches.Any(b => b.ZipCode == branch.ZipCode);
            if (duplicateZip)
            {
                ViewBag.zipcode = "A branch with this Zip Code already exists.";
            }

            var duplicateContact = db.Branches.Any(b => b.Contact == branch.Contact);
            if (duplicateContact)
            {
                ViewBag.contact1 = "A branch with this Contact number already exists.";
            }

            var duplicateAdddress = db.Branches.Any(b => b.Address == branch.Address);
            if (duplicateAdddress)
            {
                ViewBag.address = "A branch with this Address already exists.";
            }

            if (!string.IsNullOrWhiteSpace(branch.Contact) &&
                !System.Text.RegularExpressions.Regex.IsMatch(branch.Contact, @"^0\d{10}$"))
            {
                ViewBag.contact2 = "The Contact number must be exactly 11 digits and start with '0'.";
            }


            branch.BranchCode = GenerateUniqueBranchCode();

            db.Branches.Add(branch);
            db.SaveChanges();

            TempData["SuccessCreateMessage"] = "Branch Added Successfully!";

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int? id)
        {
            var branch = db.Branches.Find(id);
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Branch branch)
        {

            var duplicateZip = db.Branches.Any(b => b.ZipCode == branch.ZipCode && b.Id != branch.Id);
            if (duplicateZip)
            {
                ViewBag.zipcode = "A branch with this Zip Code already exists.";
            }

            var duplicateContact = db.Branches.Any(b => b.Contact == branch.Contact && b.Id != branch.Id);
            if (duplicateContact)
            {
                ViewBag.contact1 = "A branch with this Contact number already exists.";
            }

            var duplicateAdddress = db.Branches.Any(b => b.Address == branch.Address && b.Id != branch.Id);
            if (duplicateAdddress)
            {
                ViewBag.address = "A branch with this Address already exists.";
            }

            if (!string.IsNullOrWhiteSpace(branch.Contact) &&
                 !System.Text.RegularExpressions.Regex.IsMatch(branch.Contact, @"^0\d{10}$"))
            {
                ViewBag.contact2 = "The Contact number must be exactly 11 digits and start with '0'.";
            }

                
    

            db.Branches.Update(branch);
            db.SaveChanges();

            TempData["SuccessEditMessage"] = "Branch Edited Successfully!";

            return RedirectToAction("Index");
        }






        private string GenerateUniqueBranchCode()
        {
            string branchCode;
            Random random = new Random();
            do
            {
                branchCode = random.Next(10000000, 99999999).ToString();
            }
            while (db.Branches.Any(b => b.BranchCode == branchCode));

            return branchCode;
        }

        public IActionResult Delete(int? id)
        {
         
            var branch = db.Branches.Find(id);

           

            var relatedEmployees = db.Employees
                                      .Where(e => e.BranchId == id)
                                      .ToList();

            var relatedBookings = db.Bookings
                        .Where(b => b.BranchId == id ||
                                    relatedEmployees.Select(emp => emp.Id).Contains(b.EmployeeId ?? -1))
                        .ToList();


            if (relatedBookings.Any())
            {
                db.Bookings.RemoveRange(relatedBookings);
            }


            if (relatedEmployees.Any())
            {
                db.Employees.RemoveRange(relatedEmployees);
            }

 
            db.Branches.Remove(branch);
            db.SaveChanges();

            TempData["SuccessDeleteMessage"] = "Branch and all related records deleted successfully!";
            return RedirectToAction("Index");
        }


    }
}
