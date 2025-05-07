using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblinfographic
{
    public int Infogid { get; set; }

    public string? Infoimage { get; set; }

    public string? Infoheading { get; set; }

    public string? Infodesc { get; set; }

    public bool? Isdeleted { get; set; }

    public DateOnly? Createdon { get; set; }
}
