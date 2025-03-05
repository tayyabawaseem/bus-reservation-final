using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Refund
{
    public int RefundId { get; set; }

    public int BookingId { get; set; }

    public decimal RefundAmount { get; set; }

    public DateTime? RefundDate { get; set; }

    public string? ProcessedBy { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
