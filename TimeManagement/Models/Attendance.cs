using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class Attendance
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateOnly Date { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string Status { get; set; } = null!;

    public decimal? WorkHours { get; set; }

    public string? Notes { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User? User { get; set; }
}
