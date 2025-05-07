using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstLookup
{
    public int Lookupflag { get; set; }

    public int Lookupcode { get; set; }

    public string? Description { get; set; }

    public int Languageid { get; set; }

    public string? Hintdetails { get; set; }

    public int? Seqno { get; set; }

    public bool Active { get; set; }
}
