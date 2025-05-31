using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public string Type { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public int? RelatedId { get; set; }

    public string? RelatedType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
