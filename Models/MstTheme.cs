using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstTheme
{
    public int ThemeId { get; set; }

    public string? Theme { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Tblevent> Tblevents { get; set; } = new List<Tblevent>();

    public virtual ICollection<Tblresource> Tblresources { get; set; } = new List<Tblresource>();
}
