using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class RoleMenu
{
    public int RoleMenuId { get; set; }

    public int? RoleId { get; set; }

    public int? MenuId { get; set; }

    public bool? Display { get; set; }

    public bool? AddNew { get; set; }

    public bool? Edit { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual MstMenu? Menu { get; set; }

    public virtual MstRole? Role { get; set; }
}
