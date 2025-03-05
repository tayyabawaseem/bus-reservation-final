using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Admin")]
    public class RouteController : Controller
    {
        private readonly BusReservationSystemContext db;

        public RouteController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var routes = db.Routes
                   .Include(r => r.StartingPlaceNavigation)  // Eagerly load StartingPlaceNavigation (Location)
                   .Include(r => r.DestinationPlaceNavigation)  // Eagerly load DestinationPlaceNavigation (Location)
                   .OrderByDescending(l => l.RouteId)
                   .ToList();
            return View(routes);
        }

        public IActionResult Create()
        {

            var locations = db.Locations.Select(b => new SelectListItem
            {
                Value = b.LocationId.ToString(),
                Text = b.LocationName
            }).ToList();

            ViewBag.Locations = locations;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(bus_reservation.Models.Route route)
        {
            
            var existingRoute = db.Routes.FirstOrDefault(r => r.RouteName == route.RouteName &&
                                                              r.StartingPlace == route.StartingPlace &&
                                                              r.DestinationPlace == route.DestinationPlace);

            if (existingRoute != null)
            {
               
                if (existingRoute.RouteName == route.RouteName)
                {
                    ViewBag.routename =  "A route with this name already exists.";
                }
 
                if (existingRoute.StartingPlace == route.StartingPlace && existingRoute.DestinationPlace == route.DestinationPlace)
                {
                    ViewBag.startingplace = "A route with this starting place and destination place already exists.";
                    ViewBag.destinationplace = "A route with this starting place and destination place already exists.";
                }

                var locations = db.Locations.Select(b => new SelectListItem
                {
                    Value = b.LocationId.ToString(),
                    Text = b.LocationName
                }).ToList();

                ViewBag.Locations = locations;

                return View(route);
            }

                db.Routes.Add(route);
                db.SaveChanges();
            TempData["SuccessCreateMessage"] = "Route Added Successfully!";
            return RedirectToAction("Index");
         

        }


        public IActionResult Edit(int? id)
        {

            var locations = db.Locations.Select(b => new SelectListItem
            {
                Value = b.LocationId.ToString(),
                Text = b.LocationName
            }).ToList();

            ViewBag.Locations = locations;

            var route = db.Routes.Find(id);
            return View(route);

            return View(route);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(bus_reservation.Models.Route route)
        {
            // Check for an existing route excluding the current route's ID
            var existingRoute = db.Routes.FirstOrDefault(r => r.RouteName == route.RouteName &&
                                                              r.StartingPlace == route.StartingPlace &&
                                                              r.DestinationPlace == route.DestinationPlace &&
                                                              r.RouteId != route.RouteId); // Assuming Id is the primary key

            if (existingRoute != null)
            {
                if (existingRoute.RouteName == route.RouteName)
                {
                    ViewBag.routename = "A route with this name already exists.";
                }

                if (existingRoute.StartingPlace == route.StartingPlace && existingRoute.DestinationPlace == route.DestinationPlace)
                {
                    ViewBag.startingplace = "A route with this starting place and destination place already exists.";
                    ViewBag.destinationplace = "A route with this starting place and destination place already exists.";
                }

                // Re-populate locations for the view
                var locations = db.Locations.Select(b => new SelectListItem
                {
                    Value = b.LocationId.ToString(),
                    Text = b.LocationName
                }).ToList();

                ViewBag.Locations = locations;

                return View(route);
            }

            // If no duplicate exists, update the route
            db.Routes.Update(route);
            db.SaveChanges();
            TempData["SuccessEditMessage"] = "Route Edited Successfully!";
            return RedirectToAction("Index");
        }



        public IActionResult Delete(int? id)
        {

            var route = db.Routes.Find(id);

            var relatedBuses = db.Buses
                                 .Where(b => b.RouteId == id)
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

            db.Routes.Remove(route);
            db.SaveChanges();

            TempData["SuccessDeleteMessage"] = "Route and all related records deleted successfully!";
            return RedirectToAction("Index");
        }


    }
}
