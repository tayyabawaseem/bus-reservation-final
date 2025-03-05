using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string LocationName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Enquiry> EnquiryDestinationPlaceNavigations { get; set; } = new List<Enquiry>();

    public virtual ICollection<Enquiry> EnquiryStartingPlaceNavigations { get; set; } = new List<Enquiry>();

    public virtual ICollection<Route> RouteDestinationPlaceNavigations { get; set; } = new List<Route>();

    public virtual ICollection<Route> RouteStartingPlaceNavigations { get; set; } = new List<Route>();
}
