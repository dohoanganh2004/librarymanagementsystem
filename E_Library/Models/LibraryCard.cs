using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class LibraryCard
{
    public int CardId { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public DateOnly CardExpiryDate { get; set; }

    public int ReaderId { get; set; }

    public bool? Status { get; set; }

    public virtual Reader Reader { get; set; } = null!;
}
