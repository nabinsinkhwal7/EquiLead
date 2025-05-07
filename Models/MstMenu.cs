using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstMenu
{
    public int MenuId { get; set; }

    public string? Menu { get; set; }

    public int? MenuType { get; set; }

    public string? Controller { get; set; }

    public string? Action { get; set; }

    public int? MenuParentId { get; set; }

    public int? MenuSequence { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? StyleClass { get; set; }

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
