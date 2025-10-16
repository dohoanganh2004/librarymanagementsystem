using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Reader
{
    public int ReaderId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly? DoB { get; set; }

    public int? Age { get; set; }

    public string? Address { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public string? Avatar { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual ICollection<LibraryCard> LibraryCards { get; set; } = new List<LibraryCard>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
