using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Complaint
{
    public int Id { get; set; }

    public string PassengerName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }
}
