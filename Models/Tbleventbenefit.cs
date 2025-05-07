using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tbleventbenefit
{
    public int Eventbenefitid { get; set; }

    public int Eventid { get; set; }

    public string? Eventbenefit { get; set; }

    public virtual Tblevent Event { get; set; } = null!;
}
