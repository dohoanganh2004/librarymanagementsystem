using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int? PublicationYear { get; set; }

    public int? PublisherId { get; set; }

    public int? CategoryId { get; set; }

    public int? AuthorId { get; set; }

    public string? Image { get; set; }

    public string? Description { get; set; }

    public int? PageCount { get; set; }

    public string? Language { get; set; }

    public int? Quantity { get; set; }

    public bool? Status { get; set; }

    public virtual Author? Author { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
