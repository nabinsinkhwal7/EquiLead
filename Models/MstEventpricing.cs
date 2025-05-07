using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstEventpricing
{
    public int Eventpricingtypeid { get; set; }

    public string? Eventpricingtype { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Tblevent> Tblevents { get; set; } = new List<Tblevent>();
}
