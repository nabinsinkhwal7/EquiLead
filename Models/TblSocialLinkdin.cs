using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class TblSocialLinkdin
{
    public int ScLinkdinId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Post { get; set; }

    public string? Organization { get; set; }

    public string? Linkdinlink { get; set; }

    public int? CreatedBy { get; set; }

    public DateOnly? CreatedOn { get; set; }

    public string? Photo { get; set; }

    public bool? IsDeleted { get; set; }
}
