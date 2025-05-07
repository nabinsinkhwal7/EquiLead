using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblevidence
{
    public int Evidenceid { get; set; }

    public int Eventid { get; set; }

    public string? Evidencepath { get; set; }

    public virtual Tblevent Event { get; set; } = null!;
}
