using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantVolunteerExperience
{
    public int VolunteerId { get; set; }

    public int ApplicantId { get; set; }

    public string OrganizationName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Description { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
