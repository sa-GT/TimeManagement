using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Role { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Department { get; set; }

    public string? Position { get; set; }

    public DateOnly? JoiningDate { get; set; }

    public string? ProfilePicture { get; set; }

    public string? Status { get; set; }

    public string? LanguagePreference { get; set; }

    public string? Timezone { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public virtual ICollection<Attendance> AttendanceApprovedByNavigations { get; set; } = new List<Attendance>();

    public virtual ICollection<Attendance> AttendanceUsers { get; set; } = new List<Attendance>();

    public virtual ICollection<LeaveRequest> LeaveRequestApprovedByNavigations { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRequest> LeaveRequestUsers { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Task> TaskAssignedToNavigations { get; set; } = new List<Task>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<Task> TaskCreatedByNavigations { get; set; } = new List<Task>();

    public virtual ICollection<TimeEntry> TimeEntryApprovedByNavigations { get; set; } = new List<TimeEntry>();

    public virtual ICollection<TimeEntry> TimeEntryUsers { get; set; } = new List<TimeEntry>();
}
