using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class Publisher
{
    public int PublisherId { get; set; }

    public string PublisherName { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
