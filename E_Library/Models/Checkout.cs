using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Checkout
{
    public int CheckoutId { get; set; }

    public int BookId { get; set; }

    public int ReaderId { get; set; }

    public DateOnly BrowseDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual Reader Reader { get; set; } = null!;
}
