using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblsuccesstest
{
    public int Sstid { get; set; }

    public string? Name { get; set; }

    public string? Photo { get; set; }

    public string? Linkedin { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Organization { get; set; }
}
