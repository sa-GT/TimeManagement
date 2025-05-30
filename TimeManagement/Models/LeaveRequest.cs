using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string LeaveType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int TotalDays { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User? User { get; set; }
}
