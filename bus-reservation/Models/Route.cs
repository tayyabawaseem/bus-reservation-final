using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public string RouteName { get; set; } = null!;

    public int StartingPlace { get; set; }

    public int DestinationPlace { get; set; }

    public int Distance { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Bus> Buses { get; set; } = new List<Bus>();

    public virtual Location DestinationPlaceNavigation { get; set; } = null!;

    public virtual Location StartingPlaceNavigation { get; set; } = null!;
}
