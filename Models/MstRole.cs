using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstRole
{
    public int RoleId { get; set; }

    public string? Role { get; set; }

    public int? RoleType { get; set; }

    public int? Seq { get; set; }

    public int? IsDeleted { get; set; }

    public int? LangId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual Language? Lang { get; set; }

    public virtual ICollection<MstUser> MstUsers { get; set; } = new List<MstUser>();

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
