﻿using System;
using System.Collections.Generic;

namespace TimeManagement.Models;

public partial class ProjectDocument
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Project? Project { get; set; }
}
