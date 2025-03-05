using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace bus_reservation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

    [Authorize(Roles = "Employee,Admin")]
    public class BookingController : Controller
    {

        private readonly BusReservationSystemContext db;

        public BookingController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            var bookings = await db.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation) // Include Starting Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation) // Include Destination Location
                .Include(b => b.Employee)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A",
                    BookingDate = b.BookingDate,
                    SeatNumber = b.SeatNumber,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    Arrivaltime = b.Bus.ArrivalTime,
                    Departuretime = b.Bus.DepartureTime,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName, // Fetch starting place name
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName, // Fetch destination place name
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .OrderByDescending(l => l.BookingId).ToListAsync();

            return View(bookings);
        }

        public IActionResult Create()
        {
            var now = DateTime.Now; // Get current date and time

            var buses = (from b in db.Buses
                         join bt in db.BusTypes on b.BusTypeId equals bt.BusTypeId
                         join r in db.Routes on b.RouteId equals r.RouteId
                         join startLoc in db.Locations on r.StartingPlace equals startLoc.LocationId
                         join destLoc in db.Locations on r.DestinationPlace equals destLoc.LocationId
                         where b.AvailableSeats > 0
                               && (b.DepartureTime.Date > now.Date || // For future dates
                                   (b.DepartureTime.Date == now.Date && b.DepartureTime.TimeOfDay >= now.TimeOfDay)) // For today, check time as well
                         select new
                         {
                             b.BusId,
                             BusDetails = $"{b.BusNumber} - {bt.BusTypeName} - {r.RouteName} ({startLoc.LocationName} to {destLoc.LocationName}) - " +
                                          $"{b.DepartureTime.ToString("dd/MM/yyyy hh:mm tt")} to {b.ArrivalTime.ToString("dd/MM/yyyy hh:mm tt")} - Available Seats: {b.AvailableSeats}"
                         })
                         .ToList();

            ViewBag.BusId = new SelectList(buses, "BusId", "BusDetails");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            // Get the currently logged-in user's role from the claims
            var role = User.FindFirstValue(ClaimTypes.Role); // This will give "Admin" or "Employee"
            var employeeId = User.FindFirstValue("EmployeeID");
            var adminId = User.FindFirstValue("AdminID");
            var branchId = User.FindFirstValue("BranchID");

            // Set the status and booking date (optional as default is set in DB)
            booking.StatusBooking = "Confirmed";
            booking.BookingDate = DateTime.Now;

            // Check if the model is valid
            //if (ModelState.IsValid)
            //{
                // Get the bus details from the database
                var bus = db.Buses.FirstOrDefault(b => b.BusId == booking.BusId);
                if (bus != null)
                {
                    // Check if there are available seats
                    if (bus.AvailableSeats > 0)
                    {
                        // Assign the next available seat number to the booking
                        booking.SeatNumber = bus.TotalSeats - bus.AvailableSeats + 1;

                        // Update the bus's available seats
                        bus.AvailableSeats--;

                        // Calculate the fare
                        var route = db.Routes.FirstOrDefault(r => r.RouteId == bus.RouteId);
                        if (route != null)
                        {
                            // Fetch distance from the route
                            var distance = route.Distance; // Assuming 'Distance' is a property in the Route table

                            // Fetch bus type
                            var busType = db.BusTypes.FirstOrDefault(bt => bt.BusTypeId == bus.BusTypeId);
                            if (busType != null)
                            {
                                // Calculate fare based on bus type
                                decimal farePerKm = 2; // Base fare for Express
                                switch (busType.BusTypeName)
                                {
                                    case "Luxury":
                                        farePerKm = 5; // Add suitable fare for Luxury
                                        break;
                                    case "Volvo (Non A/C)":
                                        farePerKm = 7; // Add suitable fare for Volvo (Non A/C)
                                        break;
                                    case "Volvo (A/C)":
                                        farePerKm = 12; // Add suitable fare for Volvo (A/C)
                                        break;
                                }

                                // Calculate total fare based on distance
                                decimal totalFare = farePerKm * distance;

                                // Adjust total fare based on customer's age
                                if (booking.CustomerAge <= 5)
                                {
                                    totalFare = 0; // No charge for age <= 5
                                }
                                else if (booking.CustomerAge > 5 && booking.CustomerAge <= 12)
                                {
                                    totalFare *= 0.5m; // 50% charge for age 5 to 12
                                }
                                else if (booking.CustomerAge > 50)
                                {
                                    totalFare *= 0.7m; // 30% discount for age > 50
                                }

                                // Set the calculated fare to the booking
                                booking.Fare = (int)totalFare; // Assuming Fare is an INT type in Booking table

                                // Set Employee/Branch/Admin ID based on the role
                                if (role == "Employee")
                                {
                                    booking.BranchId = branchId != null ? int.Parse(branchId) : (int?)null;
                                    booking.EmployeeId = employeeId != null ? int.Parse(employeeId) : (int?)null;
                                }
                                else if (role == "Admin")
                                {
                                    booking.AdminId = adminId != null ? int.Parse(adminId) : (int?)null;
                                }

                                // Save the booking to the database
                                db.Bookings.Add(booking);
                                db.SaveChanges();

                            return RedirectToAction("Ticket", new { id = booking.BookingId }); 
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Route not found.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No available seats on this bus.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bus not found.");
                }
            //}

            // If there's an issue with the model, return the same view to fix errors
            return View(booking);
        }


        public async Task<IActionResult> Ticket(int id)
        {
            var booking = await db.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation) // Include Starting Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation) // Include Destination Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.BusType) // Include Bus Type
                .Include(b => b.Employee)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A", // Show "N/A" if email is null
                    BookingDate = b.BookingDate,
                    Arrivaltime = b.Bus.ArrivalTime,
                    Departuretime = b.Bus.DepartureTime,
                    SeatNumber = b.SeatNumber,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName, // Fetch starting place name
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName, // Fetch destination place name
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .FirstOrDefaultAsync(b => b.BookingId == id);


            return View(booking); // Return the booking details to the Ticket view
        }

        public async Task<IActionResult> Details(int id)
        {
            var booking = await db.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation) // Include Starting Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation) // Include Destination Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.BusType) // Include Bus Type
                .Include(b => b.Employee)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A", // Show "N/A" if email is null
                    BookingDate = b.BookingDate,
                    Arrivaltime = b.Bus.ArrivalTime,
                    RefundAmount = b.RefundAmount ?? 0,
                    Departuretime = b.Bus.DepartureTime,
                    SeatNumber = b.SeatNumber,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName, // Fetch starting place name
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName, // Fetch destination place name
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .FirstOrDefaultAsync(b => b.BookingId == id);


            return View(booking); // Return the booking details to the Ticket view
        }



        public IActionResult Cancel(int? id)
        {
            // Check if the id is null
            if (id == null)
            {
                return NotFound(); // or handle the error accordingly
            }

            // Find the booking using the provided id, including associated Bus
            var booking = db.Bookings.Include(b => b.Bus).FirstOrDefault(b => b.BookingId == id);

            // Check if the booking or associated bus is found
            if (booking == null || booking.Bus == null)
            {
                return NotFound(); // Handle booking or bus not found
            }

            // Get current date and time
            var today = DateTime.Now.Date;

            // Get the departure time from the associated Bus
            var bookingDate = booking.Bus.DepartureTime.Date;

            // Calculate the difference in days between the departure date and today
            var daysDifference = (bookingDate - today).TotalDays;

            // Calculate refund based on days left to the booking date
            double refundPercentage = 0;

            // Apply cancellation rules based on daysDifference
            if (daysDifference >= 2)  // More than 2 days left
            {
                refundPercentage = 100; // Full refund
            }
            else if (daysDifference >= 1 && daysDifference < 2)  // 1 day left
            {
                refundPercentage = 85; // 85% refund, 15% deducted
            }
            else if (daysDifference < 1)  // Same day cancellation
            {
                refundPercentage = 70; // 70% refund, 30% deducted
            }

            // Calculate refund amount based on the fare and the percentage
            double refundAmount = (booking.Fare * refundPercentage) / 100;

            // Round the refund amount to the nearest integer
            int roundedRefundAmount = (int)Math.Round(refundAmount, MidpointRounding.AwayFromZero);

            // Update booking status and save the rounded refund amount
            booking.StatusBooking = "Cancelled";
            booking.RefundAmount = roundedRefundAmount; // Storing as int

            // Update available seats in the associated bus
            booking.Bus.AvailableSeats += 1; // Increase available seats by 1

            // Save the changes to both the booking and the bus
            db.Update(booking);
            db.Update(booking.Bus); // Update the Bus entity to reflect the new available seats
            db.SaveChanges();

            // Redirect to a page that shows canceled bookings and refund details
            return RedirectToAction("CancelledBookings");
        }



        public async Task<IActionResult> CancelledBookings()
        {
            var cancelledBookings = await db.Bookings
                .Where(b => b.StatusBooking == "Cancelled")
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation)
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A",
                    BookingDate = b.BookingDate,
                    Arrivaltime = b.Bus.ArrivalTime,
                    Departuretime = b.Bus.DepartureTime,
                    SeatNumber = b.SeatNumber,
                    RefundAmount = b.RefundAmount ?? 0,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName,
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName,
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .OrderByDescending(l => l.BookingId).ToListAsync();

            return View(cancelledBookings);
        }

        public async Task<IActionResult> ConfirmedBookings()
        {
            var confirmedBookings = await db.Bookings
                .Where(b => b.StatusBooking == "Confirmed")
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation)
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation)
                .Include(b => b.Employee)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A",
                    BookingDate = b.BookingDate,
                    Arrivaltime = b.Bus.ArrivalTime,
                    Departuretime = b.Bus.DepartureTime,
                    SeatNumber = b.SeatNumber,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName,
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName,
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .OrderByDescending(l => l.BookingId).ToListAsync();

            return View(confirmedBookings);
        }

        public IActionResult Delete(int? id)
        {
            var booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            TempData["SuccessDeleteMessage"] = "Booking Deleted Successfully!";
            return RedirectToAction("Index");
        }

    }
}
