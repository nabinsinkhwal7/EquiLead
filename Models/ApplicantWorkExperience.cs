using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantWorkExperience
{
    public int WorkExperienceId { get; set; }

    public int ApplicantId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string Industry { get; set; } = null!;

    public string EmploymentType { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? KeyResponsibilities { get; set; }

    public string? LeadershipImpact { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
