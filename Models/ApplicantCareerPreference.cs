using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantCareerPreference
{
    public int PreferenceId { get; set; }

    public int ApplicantId { get; set; }

    public string? PreferredJobRole { get; set; }

    public string? IndustriesOfInterest { get; set; }

    public string? EmploymentTypePreference { get; set; }

    public string? LeadershipAspirations { get; set; }

    public string? PreferredJobLocation { get; set; }

    public string? WillingToRelocate { get; set; }

    public short? WorkMode { get; set; }

    public List<int>? PreferredSector { get; set; }

    public List<int>? FunctionalArea { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
