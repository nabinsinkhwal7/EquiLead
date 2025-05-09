using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ResourceClickLog
{
    public int Id { get; set; }

    public int ResourceId { get; set; }

    public int? UserId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime ClickedAt { get; set; }
}
