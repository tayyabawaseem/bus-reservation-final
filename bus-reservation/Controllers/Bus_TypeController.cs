using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Admin")]
    public class Bus_TypeController : Controller
    {
        private readonly BusReservationSystemContext db;

        public Bus_TypeController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var Bustypes = db.BusTypes.OrderByDescending(l => l.BusTypeId).ToList(); // Ensure this is not null
            return View(Bustypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BusType Busname)
        {
            bool isDuplicate = db.BusTypes.Any(b => b.BusTypeName.ToLower() == Busname.BusTypeName.ToLower());

            if (isDuplicate)
            {
                ModelState.AddModelError("BusTypeName", "Bus Type already exists.");

                return View(Busname);
            }

            db.BusTypes.Add(Busname);
            db.SaveChanges();

            TempData["SuccessCreateMessage"] = "Bus Type Created Successfully!";

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int? id)
        {
            var bustype = db.BusTypes.Find(id);
            return View(bustype);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BusType Busname)
        {
            bool isDuplicate = db.BusTypes
                .Any(b => b.BusTypeName.ToLower() == Busname.BusTypeName.ToLower() && b.BusTypeId != Busname.BusTypeId);

            if (isDuplicate)
            {
                ModelState.AddModelError("BusTypeName", "Bus Type already exists.");

                return View(Busname);
            }

            db.BusTypes.Update(Busname);
            db.SaveChanges();

            TempData["SuccessEditMessage"] = "Bus Type Edited Successfully!";

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int? id)
        {

            var bustype = db.BusTypes.Find(id);

            var relatedBuses = db.Buses
                                 .Where(b => b.BusTypeId == id)
                                 .ToList();

            var relatedBookings = db.Bookings
                                    .Where(b => relatedBuses.Select(bus => bus.BusId).Contains(b.BusId))
                                    .ToList();

            if (relatedBookings.Any())
            {
                db.Bookings.RemoveRange(relatedBookings);
            }

            if (relatedBuses.Any())
            {
                db.Buses.RemoveRange(relatedBuses);
            }

            db.BusTypes.Remove(bustype);
            db.SaveChanges();

            TempData["SuccessDeleteMessage"] = "BusType and all related records deleted successfully!";
            return RedirectToAction("Index");
        }


    }
}
