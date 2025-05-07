using System;
using System.Collections.Generic;

namespace EquidCMS.Models;

public partial class Applicant
{
    public int ApplicantId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? CountryCode { get; set; }

    public string? PhoneNumber { get; set; }

    public string? LinkedinProfile { get; set; }

    public string? Location { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Gender { get; set; }

    public string? ReferenceCode { get; set; }

    public bool? IsMigrated { get; set; }

    public virtual ApplicantCareerPreference? ApplicantCareerPreference { get; set; }

    public virtual ICollection<ApplicantCertificationTraning> ApplicantCertificationTranings { get; set; } = new List<ApplicantCertificationTraning>();

    public virtual ICollection<ApplicantEducation> ApplicantEducations { get; set; } = new List<ApplicantEducation>();

    public virtual ICollection<ApplicantLanguage> ApplicantLanguages { get; set; } = new List<ApplicantLanguage>();

    public virtual ApplicantProfile? ApplicantProfile { get; set; }

    public virtual ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    public virtual ICollection<ApplicantVolunteerExperience> ApplicantVolunteerExperiences { get; set; } = new List<ApplicantVolunteerExperience>();

    public virtual ICollection<ApplicantWorkExperience> ApplicantWorkExperiences { get; set; } = new List<ApplicantWorkExperience>();

    public virtual ICollection<ReferralCode> ReferralCodes { get; set; } = new List<ReferralCode>();
}
