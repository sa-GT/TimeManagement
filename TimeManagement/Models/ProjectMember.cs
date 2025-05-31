using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class ProjectMember
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public string Role { get; set; } = null!;

    public decimal? HourlyRate { get; set; }

    public DateOnly? JoinDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
