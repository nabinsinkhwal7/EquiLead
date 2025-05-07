using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantSkill
{
    public int SkillId { get; set; }

    public int ApplicantId { get; set; }

    public string SkillName { get; set; } = null!;

    public short SkillType { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
