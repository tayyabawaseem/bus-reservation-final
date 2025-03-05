namespace bus_reservation.Models
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public int? CustomerAge { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime BookingDate { get; set; }

        public DateTime Arrivaltime { get; set; }

        public DateTime Departuretime { get; set; }
        public int SeatNumber { get; set; }
        public int Fare { get; set; }
        public string StatusBooking { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RefundAmount { get; set; }
        public string BusCode { get; set; }
        public string BusTypeName { get; set; }
        public string RouteName { get; set; }
        public string StartingPlace { get; set; }
        public string DestinationPlace { get; set; }
        public int Distance { get; set; }
        public string BookingManagedBy { get; set; } // This will hold either Employee details or "Admin"
    }

}
