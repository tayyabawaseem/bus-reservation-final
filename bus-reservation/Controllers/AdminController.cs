using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly BusReservationSystemContext db;

        public AdminController(BusReservationSystemContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today.Date;
            var tomorrow = today.AddDays(1).Date;

            var dashboardViewModel = new DashboardViewModel
            {
                TotalBuses = db.Buses.Count(),
                TotalRoutes = db.Routes.Count(),
                TotalBranches = db.Branches.Count(),
                TotalEmployees = db.Employees.Count(),
                TotalBusTypes = db.BusTypes.Count(),
                TotalLocations = db.Locations.Count(),
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
    }
}
