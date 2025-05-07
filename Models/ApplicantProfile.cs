using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantProfile
{
    public int ProfileId { get; set; }

    public int ApplicantId { get; set; }

    public string? Pronouns { get; set; }

    public string? PersonalWebsite { get; set; }

    public string? Headline { get; set; }

    public string? ShortBio { get; set; }

    public short? JobSearchStatus { get; set; }

    public string? DesiredSalaryRange { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? InternationalExperience { get; set; }

    public string? CommunityAdvocacyExperience { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
