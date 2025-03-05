using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Admin")]
    public class LocationController : Controller
    {

        private readonly BusReservationSystemContext db;

        public LocationController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var locations = db.Locations
                .OrderByDescending(l => l.LocationId)
                .ToList();
            return View(locations);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Location location)
        {
            // Check if the location name already exists
            bool locationExists = db.Locations.Any(l => l.LocationName == location.LocationName);

            if (locationExists)
            {
                // Add a model error if the location name already exists
                ModelState.AddModelError("LocationName", "Location name already exists.");

                // Return the view with the model to display validation errors
                return View(location);
            }

            if (ModelState.IsValid)
            {
                db.Locations.Add(location);
                db.SaveChanges();

                TempData["SuccessCreateMessage"] = "Location Added Successfully!";
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with errors
            return View(location);
        }


        public IActionResult Edit(int? id)
        {
            var Location = db.Locations.Find(id);
            return View(Location);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Location location)
        {
            // Check if the new name already exists in the database, excluding the current location being edited
            bool locationExists = db.Locations.Any(l => l.LocationName == location.LocationName && l.LocationId != location.LocationId);

            if (locationExists)
            {
                // Add a model error if the location name already exists (excluding the current location)
                ModelState.AddModelError("LocationName", "Location name already exists.");

                // Return the view with the model to display validation errors
                return View(location);
            }

            if (ModelState.IsValid)
            {
                db.Locations.Update(location);
                db.SaveChanges();

                TempData["SuccessEditMessage"] = "Location Edited Successfully!";
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with errors
            return View(location);
        }


        public IActionResult Delete(int id)
        {
            var location = db.Locations.Find(id);

            if (location == null)
            {
                TempData["ErrorMessage"] = "Location not found.";
                return RedirectToAction("Index");
            }

            var relatedEnquiries = db.Enquiries
                             .Where(e => e.StartingPlace == id || e.DestinationPlace == id)
                             .ToList();

            var relatedRoutes = db.Routes
                                  .Where(r => r.StartingPlace == id || r.DestinationPlace == id)
                                  .ToList();

 
            var relatedBuses = db.Buses
                                 .Where(b => relatedRoutes.Select(r => r.RouteId).Contains(b.RouteId))
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


            if (relatedRoutes.Any())
            {
                db.Routes.RemoveRange(relatedRoutes);
            }

            if (relatedEnquiries.Any())
            {
                db.Enquiries.RemoveRange(relatedEnquiries);
            }

            db.Locations.Remove(location);
            db.SaveChanges();

            TempData["SuccessDeleteMessage"] = "Location and all related records deleted successfully!";
            return RedirectToAction("Index");
        }



    }
}
