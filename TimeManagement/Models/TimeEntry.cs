using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class TimeEntry
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? TaskId { get; set; }

    public int? ProjectId { get; set; }

    public DateOnly Date { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal Hours { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public bool? Billable { get; set; }

    public decimal? BillingRate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Task? Task { get; set; }

    public virtual User? User { get; set; }
}
