using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Models;

public partial class BusReservationSystemContext : DbContext
{
    public BusReservationSystemContext()
    {
    }

    public BusReservationSystemContext(DbContextOptions<BusReservationSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Bus> Buses { get; set; }

    public virtual DbSet<BusType> BusTypes { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Enquiry> Enquiries { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=TAYYABAWASEEM\\SQLEXPRESS;Initial Catalog=Bus_Reservation_System;User ID=TW;Password=12345678;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admin__3214EC2796B146A4");

            entity.ToTable("Admin");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__35ABFDC02AF07D93");

            entity.Property(e => e.BookingId).HasColumnName("Booking_Id");
            entity.Property(e => e.AdminId).HasColumnName("Admin_Id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Booking_Date");
            entity.Property(e => e.BranchId).HasColumnName("Branch_Id");
            entity.Property(e => e.BusId).HasColumnName("Bus_Id");
            entity.Property(e => e.ConfirmationSent).HasDefaultValueSql("((0))");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.CustomerAge).HasColumnName("Customer_Age");
            entity.Property(e => e.CustomerContact)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Customer_Contact");
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Customer_Email");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Customer_Name");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");
            entity.Property(e => e.SeatNumber).HasColumnName("Seat_Number");
            entity.Property(e => e.StatusBooking)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('Confirmed')")
                .HasColumnName("Status_Booking");

            entity.HasOne(d => d.Admin).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_Ticket_Booking_Admin");

            entity.HasOne(d => d.Branch).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Ticket_Booking_Branch");

            entity.HasOne(d => d.Bus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Booking_Bus");

            entity.HasOne(d => d.Employee).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Ticket_Booking_Employee");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Branch__3213E83F86AA041C");

            entity.ToTable("Branch");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("branch_code");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contact");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__Bus__B0524D19EC3B18F9");

            entity.ToTable("Bus");

            entity.Property(e => e.BusId).HasColumnName("Bus_Id");
            entity.Property(e => e.ArrivalTime)
                .HasColumnType("datetime")
                .HasColumnName("Arrival_Time");
            entity.Property(e => e.AvailableSeats).HasColumnName("Available_Seats");
            entity.Property(e => e.BusImage)
                .IsUnicode(false)
                .HasColumnName("Bus_Image");
            entity.Property(e => e.BusNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Bus_Number");
            entity.Property(e => e.BusTypeId).HasColumnName("Bus_Type_Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("datetime")
                .HasColumnName("Departure_Time");
            entity.Property(e => e.RouteId).HasColumnName("Route_Id");
            entity.Property(e => e.TotalSeats).HasColumnName("Total_Seats");

            entity.HasOne(d => d.BusType).WithMany(p => p.Buses)
                .HasForeignKey(d => d.BusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bus_ToBusType");

            entity.HasOne(d => d.Route).WithMany(p => p.Buses)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bus_ToRoute");
        });

        modelBuilder.Entity<BusType>(entity =>
        {
            entity.HasKey(e => e.BusTypeId).HasName("PK__Bus_Type__A2B1F2DBC8C6F39F");

            entity.ToTable("Bus_Type");

            entity.Property(e => e.BusTypeId).HasColumnName("Bus_Type_Id");
            entity.Property(e => e.BusTypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Bus_Type_Name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Complain__3214EC07001A09CC");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PassengerName).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("('Pending')");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0716877FA3");

            entity.ToTable("Employee");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BranchId).HasColumnName("Branch_Id");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.Designation)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("designation");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeImage)
                .IsUnicode(false)
                .HasColumnName("employee_image");
            entity.Property(e => e.Fathername)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MartialStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("martial_status");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Salary).HasColumnName("salary");

            entity.HasOne(d => d.Branch).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Branch");
        });

        modelBuilder.Entity<Enquiry>(entity =>
        {
            entity.HasKey(e => e.EnquiryId).HasName("PK__Enquirie__57CC01B3D53BB0B4");

            entity.Property(e => e.EnquiryId).HasColumnName("enquiry_id");
            entity.Property(e => e.DestinationPlace).HasColumnName("destination_place");
            entity.Property(e => e.StartingPlace).HasColumnName("starting_place");
            entity.Property(e => e.TravelDate)
                .HasColumnType("datetime")
                .HasColumnName("travel_date");

            entity.HasOne(d => d.DestinationPlaceNavigation).WithMany(p => p.EnquiryDestinationPlaceNavigations)
                .HasForeignKey(d => d.DestinationPlace)
                .HasConstraintName("FK_Routes_ToDestinationPlaceSearch");

            entity.HasOne(d => d.StartingPlaceNavigation).WithMany(p => p.EnquiryStartingPlaceNavigations)
                .HasForeignKey(d => d.StartingPlace)
                .HasConstraintName("FK_Routes_ToStartingPlaceSearch");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__D2BA00E2A5FAA06C");

            entity.ToTable("Location");

            entity.Property(e => e.LocationId).HasColumnName("Location_Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.LocationName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Location_Name");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__Refunds__725AB920D41676E0");

            entity.Property(e => e.ProcessedBy).HasMaxLength(100);
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RefundDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Refunds__Booking__3587F3E0");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Routes__80979B4DF4D51A9C");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Distance).HasColumnName("distance");
            entity.Property(e => e.RouteName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.DestinationPlaceNavigation).WithMany(p => p.RouteDestinationPlaceNavigations)
                .HasForeignKey(d => d.DestinationPlace)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Routes_ToDestinationPlace");

            entity.HasOne(d => d.StartingPlaceNavigation).WithMany(p => p.RouteStartingPlaceNavigations)
                .HasForeignKey(d => d.StartingPlace)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Routes_ToStartingPlace");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
