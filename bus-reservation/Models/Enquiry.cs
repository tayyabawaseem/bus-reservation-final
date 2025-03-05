using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Enquiry
{
    public int EnquiryId { get; set; }

    public int? StartingPlace { get; set; }

    public int? DestinationPlace { get; set; }

    public DateTime? TravelDate { get; set; }

    public virtual Location? DestinationPlaceNavigation { get; set; }

    public virtual Location? StartingPlaceNavigation { get; set; }
}
