using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Tblourteam
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Photo { get; set; }

    public string? Linkedin { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }

    public string? Designation { get; set; }

    public string? Organization { get; set; }
}
