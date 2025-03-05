using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int CustomerAge { get; set; }

    public string CustomerContact { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public DateTime BookingDate { get; set; }

    public int BusId { get; set; }

    public int SeatNumber { get; set; }

    public int Fare { get; set; }

    public int? BranchId { get; set; }

    public int? EmployeeId { get; set; }

    public int? RefundAmount { get; set; }

    public int? AdminId { get; set; }

    public string StatusBooking { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool? ConfirmationSent { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Bus Bus { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
