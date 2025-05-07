using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblcompany
{
    public int Companyid { get; set; }

    public string? Name { get; set; }

    public string? Logo { get; set; }

    public string? Overview { get; set; }

    public string? Website { get; set; }

    public string? Sociallink { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual ICollection<Tbljob> Tbljobs { get; set; } = new List<Tbljob>();
}
