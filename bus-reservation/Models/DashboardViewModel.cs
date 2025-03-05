namespace bus_reservation.Models
{
    public class DashboardViewModel
    {
        public int TotalBuses { get; set; }
        public int TotalRoutes { get; set; }
        public int TotalBranches { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalBusTypes { get; set; }
        public int TotalLocations { get; set; }

        public List<Bus> BusesToday { get; set; }

        public List<Bus> BusesTomorrow { get; set; }
    }

}
