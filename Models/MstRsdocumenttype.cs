using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstRsdocumenttype
{
    public int Rsdocumenttypeid { get; set; }

    public string? Rsdocumenttype { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Tblresource> Tblresources { get; set; } = new List<Tblresource>();
}
