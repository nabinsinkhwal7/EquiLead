using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantCertificationTraning
{
    public int CertificationTrainingId { get; set; }

    public int ApplicantId { get; set; }

    public string Name { get; set; } = null!;

    public string IssuingOrganization { get; set; } = null!;

    public int? YearEarned { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
