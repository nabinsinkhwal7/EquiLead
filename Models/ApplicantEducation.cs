using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantEducation
{
    public int EducationId { get; set; }

    public int ApplicantId { get; set; }

    public string Degree { get; set; } = null!;

    public string FieldOfStudy { get; set; } = null!;

    public string InstitutionName { get; set; } = null!;

    public int? YearOfCompletion { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
