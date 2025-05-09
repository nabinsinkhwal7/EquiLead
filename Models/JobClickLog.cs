using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class JobClickLog
{
    public int Id { get; set; }

    public int JobId { get; set; }

    public int? UserId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime ClickedAt { get; set; }
}
