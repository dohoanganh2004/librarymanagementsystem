using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Fine
{
    public int FineId { get; set; }

    public int CheckoutId { get; set; }

    public int ReaderId { get; set; }

    public decimal FineAmount { get; set; }

    public DateOnly FineDate { get; set; }

    public string? FineType { get; set; }

    public bool? Status { get; set; }

    public virtual Checkout Checkout { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;
}
