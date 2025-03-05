using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Employee,Admin")]
    public class EnquiryController : Controller
    {
        private readonly BusReservationSystemContext db;

        public EnquiryController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult CreateEnquiry()
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
        public IActionResult CreateEnquiry(Enquiry enquiry)
        {
            if (!ModelState.IsValid)
            {

                var locations = db.Locations.Select(b => new SelectListItem
                {
                    Value = b.LocationId.ToString(),
                    Text = b.LocationName
                }).ToList();

                ViewBag.Locations = locations;
                return View(enquiry);
            }


            var buses = db.Buses
    .Include(b => b.BusType) // Include BusType information
    .Where(b => (b.Route.StartingPlace == enquiry.StartingPlace || enquiry.StartingPlace == 0)
              && (b.Route.DestinationPlace == enquiry.DestinationPlace || enquiry.DestinationPlace == 0)
              && (b.DepartureTime.Date >= enquiry.TravelDate))
    .ToList();


            if (buses == null || !buses.Any())
            {
                return View("BusAvailabilityResults", new List<Bus>()); // Pass an empty list if no buses found
            }

            db.Enquiries.Add(enquiry);
            db.SaveChanges();

            return View("BusAvailabilityResults", buses);

        }

        public IActionResult BusAvailabilityResults()
        {
            return View();
        }
        

    }
}
