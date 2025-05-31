using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class ActivityLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public string? Description { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Module { get; set; }

    public int? RelatedId { get; set; }

    public string? RelatedType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
