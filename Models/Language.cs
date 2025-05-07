using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Language
{
    public int LangId { get; set; }

    public string? Lang { get; set; }

    public int? Sequence { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? IsDeleted { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<MstRole> MstRoles { get; set; } = new List<MstRole>();
}
