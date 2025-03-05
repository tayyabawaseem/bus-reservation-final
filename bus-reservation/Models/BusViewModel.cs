namespace bus_reservation.Models
{
    public class BusViewModel
    {
        public string BusNumber { get; set; }
        public string BusTypeName { get; set; }
        public string RouteName { get; set; }
        public string StartingPlace { get; set; }
        public string DestinationPlace { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }

}
