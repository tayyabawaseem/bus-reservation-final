using Microsoft.AspNetCore.Mvc.Rendering;

namespace bus_reservation.Models
{
    public class BusCreateViewModel
    {
        public Bus Bus { get; set; }
        public List<SelectListItem> BusTypes { get; set; }
        public List<SelectListItem> Routes { get; set; }
    }
}
