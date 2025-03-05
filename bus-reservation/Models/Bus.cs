using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Bus
{
    public int BusId { get; set; }

    public string BusNumber { get; set; } = null!;

    public int BusTypeId { get; set; }

    public int RouteId { get; set; }

    public int TotalSeats { get; set; }

    public int AvailableSeats { get; set; }

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public string? BusImage { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual BusType BusType { get; set; } = null!;

    public virtual Route Route { get; set; } = null!;
}
