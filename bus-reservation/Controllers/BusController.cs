using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Admin")]
    public class BusController : Controller
    {

        private readonly BusReservationSystemContext db;

        public BusController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var buses = db.Buses
                .Include(b => b.BusType)                // Include BusType
                .Include(b => b.Route)                   // Include Route
                .ThenInclude(r => r.StartingPlaceNavigation) // Include StartingPlace navigation
                .Include(r => r.Route.DestinationPlaceNavigation) // Include DestinationPlace navigation
                .OrderByDescending(l => l.BusId).ToList();

            return View(buses);
        }

        public IActionResult Details(int? id)
        {
            var buses = db.Buses
         .Include(b => b.BusType)                // Include BusType
         .Include(b => b.Route)                  // Include Route
         .ThenInclude(r => r.StartingPlaceNavigation) // Include StartingPlace navigation
         .Include(r => r.Route.DestinationPlaceNavigation) // Include DestinationPlace navigation
         .FirstOrDefault(b => b.BusId == id);    // Fetch the bus by id

            return View(buses);
        }




        public IActionResult Create()
        {

            var busTypes = db.BusTypes
                .Select(bt => new SelectListItem
                {
                    Value = bt.BusTypeId.ToString(),
                    Text = bt.BusTypeName
                }).ToList();


            var routes = db.Routes.ToList();
            var locations = db.Locations.ToList();

            var viewModel = new BusCreateViewModel
            {
                Bus = new Bus(),
                BusTypes = busTypes,
                Routes = routes.Select(r => new SelectListItem
                {
                    Value = r.RouteId.ToString(),
                    Text = $"{r.RouteName} ({locations.FirstOrDefault(l => l.LocationId == r.StartingPlace)?.LocationName} to {locations.FirstOrDefault(l => l.LocationId == r.DestinationPlace)?.LocationName})"
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bus bus, IFormFile file)
        {
            string imagename = DateTime.Now.ToString("yymmddhhmmss");
            imagename += "-" + Path.GetFileName(file.FileName);

            var imagepath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/admin/images/bus_images");
            var imageValue = Path.Combine(imagepath, imagename);

            using (var stream = new FileStream(imageValue, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var dbimage = Path.Combine("/admin/images/bus_images", imagename);

            bus.BusImage = dbimage;
            bus.BusNumber = GenerateUniqueBusNumber();
                db.Buses.Add(bus);
                db.SaveChanges();
            TempData["SuccessCreateMessage"] = "Bus Added Successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int? id)
        {

            var bus = db.Buses
                .Include(b => b.Route) 
                .FirstOrDefault(b => b.BusId == id);


            
            var busTypes = db.BusTypes
                .Select(bt => new SelectListItem
                {
                    Value = bt.BusTypeId.ToString(),
                    Text = bt.BusTypeName
                }).ToList();

   
            var routes = db.Routes.ToList();
            var locations = db.Locations.ToList();

            var viewModel = new BusCreateViewModel
            {
                Bus = bus, 
                BusTypes = busTypes,
                Routes = routes.Select(r => new SelectListItem
                {
                    Value = r.RouteId.ToString(),
                    Text = $"{r.RouteName} ({locations.FirstOrDefault(l => l.LocationId == r.StartingPlace)?.LocationName} to {locations.FirstOrDefault(l => l.LocationId == r.DestinationPlace)?.LocationName})"
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Bus bus, IFormFile file, string oldImage)
        {
            if (file != null && file.Length > 0)
            {
                string imagename = DateTime.Now.ToString("yymmddhhmmss");
                imagename += "-" + Path.GetFileName(file.FileName);

                var imagepath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/admin/images/bus_images");
                var imageValue = Path.Combine(imagepath, imagename);

                using (var stream = new FileStream(imageValue, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var dbimage = Path.Combine("/admin/images/bus_images", imagename);

                bus.BusImage = dbimage;
                db.Buses.Update(bus);
                db.SaveChanges();
            }
            else
            {
                bus.BusImage = oldImage;
                db.Buses.Update(bus);
                db.SaveChanges();
            }
            TempData["SuccessEditMessage"] = "Bus Edited Successfully!";
            return RedirectToAction("Index");
            
        }


        private string GenerateUniqueBusNumber()
        {
            Random random = new Random();
            string busNumber;

            do
            {
                // Generate a random 2-letter prefix
                string prefix = $"{GetRandomLetter()}{GetRandomLetter()}";  // Generates two random letters
                                                                            // Generate a random 3-digit number
                string number = random.Next(100, 1000).ToString();  // Generates a number between 100 and 999
                busNumber = $"{prefix}-{number}";  // Combine to form the bus number
            } while (db.Buses.Any(b => b.BusNumber == busNumber));  // Check for uniqueness

            return busNumber;
        }

        // Method to get a random letter
        private char GetRandomLetter()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";  // Uppercase letters
            Random random = new Random();
            return chars[random.Next(chars.Length)];
        }

        public IActionResult Delete(int? id)
        {
            // Check if the bus exists
            var bus = db.Buses.Include(b => b.Bookings).FirstOrDefault(b => b.BusId == id);

            if (bus == null)
            {
                return NotFound();
            }

            // Find and remove all bookings related to this bus
            var bookings = db.Bookings.Where(b => b.BusId == id).ToList();
            if (bookings.Any())
            {
                db.Bookings.RemoveRange(bookings); // Delete associated bookings
            }

            // Remove the bus after deleting the bookings
            db.Buses.Remove(bus);
            db.SaveChanges();

            TempData["SuccessDeleteMessage"] = "Bus and its associated bookings deleted successfully!";
            return RedirectToAction("Index");
        }

    }
}
