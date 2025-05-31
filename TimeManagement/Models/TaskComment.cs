using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class TaskComment
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? UserId { get; set; }

    public string? CommentText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Task? Task { get; set; }

    public virtual User? User { get; set; }
}
