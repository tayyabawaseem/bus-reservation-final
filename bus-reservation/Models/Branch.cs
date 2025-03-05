using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Branch
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
