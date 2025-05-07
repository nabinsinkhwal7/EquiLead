using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class MstUser
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Mobile { get; set; }

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public string? EmailId { get; set; }

    public string? AuthToken { get; set; }

    public string? Address { get; set; }

    public int? Pincode { get; set; }

    public int? IsActive { get; set; }

    public int? IsDeleted { get; set; }

    public DateTime? PwdLinkExpTime { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public int? NoofLogin { get; set; }

    public DateTime? LastLogin { get; set; }

    public string? Vcode { get; set; }

    public virtual MstRole Role { get; set; } = null!;
}
