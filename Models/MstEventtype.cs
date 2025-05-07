using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstEventtype
{
    public int Eventtypeid { get; set; }

    public string? Eventtype { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Tblevent> Tblevents { get; set; } = new List<Tblevent>();
}
