using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int ReaderId { get; set; }

    public int BookId { get; set; }

    public DateOnly ReservationDate { get; set; }

    public bool? Status { get; set; }

    public DateOnly? PickupDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
