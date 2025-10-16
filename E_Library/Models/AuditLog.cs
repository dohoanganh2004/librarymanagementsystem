using System;
using System.Collections.Generic;

namespace E_Library.Models;

public partial class AuditLog
{
    public int LogId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Description { get; set; }

    public string? TableName { get; set; }

    public int? RecordId { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
