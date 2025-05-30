using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class Task
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public string TaskName { get; set; } = null!;

    public string? Description { get; set; }

    public int? AssignedTo { get; set; }

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public decimal? EstimatedHours { get; set; }

    public decimal? ActualHours { get; set; }

    public int? CompletionPercentage { get; set; }

    public int? ParentTaskId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Task> InverseParentTask { get; set; } = new List<Task>();

    public virtual Task? ParentTask { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<TaskAttachment> TaskAttachments { get; set; } = new List<TaskAttachment>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
