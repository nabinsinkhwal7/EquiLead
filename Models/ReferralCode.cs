using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ReferralCode
{
    public int Id { get; set; }

    public int ApplicantId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? Count { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
