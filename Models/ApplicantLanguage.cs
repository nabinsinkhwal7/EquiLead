using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class ApplicantLanguage
{
    public int LanguageId { get; set; }

    public int ApplicantId { get; set; }

    public string Language { get; set; } = null!;

    public short ProficiencyLevel { get; set; }

    public virtual Applicant Applicant { get; set; } = null!;
}
