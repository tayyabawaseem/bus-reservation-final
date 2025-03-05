using System;
using System.Collections.Generic;

namespace bus_reservation.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Fathername { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Age { get; set; }

    public DateTime Dob { get; set; }

    public string MartialStatus { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string Qualification { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public int Salary { get; set; }

    public int BranchId { get; set; }

    public string EmployeeImage { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Branch Branch { get; set; } = null!;
}
