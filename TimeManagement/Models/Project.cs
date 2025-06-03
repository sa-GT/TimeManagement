using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public int? ManagerId { get; set; }

    public decimal? Budget { get; set; }

    public string? Currency { get; set; }

    public string? Priority { get; set; }

    public int? Progress { get; set; }

    public string? ClientName { get; set; }

    public string? ClientEmail { get; set; }

    public string? ClientPhone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Category { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; } = new List<ProjectDocument>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
