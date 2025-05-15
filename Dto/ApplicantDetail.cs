using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EquidCMS.Dto
{
    public class ApplicantProfileDto
    {
        // ========== Basic Personal Information ==========
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        public string Pronouns { get; set; }
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string? CountryCode { get; set; }
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        public string Location { get; set; } // City, Country

        public string LinkedInProfile { get; set; }

        public string PersonalWebsite { get; set; }

        public short? YearsOfExperence { get; set; }
        public string Headline { get; set; } // Short descriptive title

        public string ShortBio { get; set; } // 2-3 sentence intro

        public short? JobSearchStatus { get; set; } // Actively Looking, Open, Not Looking

        public string DesiredSalaryRange { get; set; }    
        
        // ========== New Fields for Additional Information ==========
        public string InternationalExperience { get; set; } // International Experience: Have worked/studied/lived in multiple countries
        public string CommunityAdvocacyExperience { get; set; } // Community & Advocacy Experience Highlighting Leadership in DEIB and Social Impact



        // ========== Work Experience (One-to-Many) ==========
        public List<WorkExperienceDto> WorkExperiences { get; set; } = new List<WorkExperienceDto>();

        // ========== Education (One-to-Many) ==========
        public List<EducationDto> EducationHistory { get; set; } = new List<EducationDto>();

        // ========== Certifications (One-to-Many) ==========
        public List<CertificationDto> Certifications { get; set; } = new List<CertificationDto>();

        // ========== Skills (One-to-Many) ==========
        public List<SkillDto> Skills { get; set; } = new List<SkillDto>();

        // ========== Languages (One-to-Many) ==========
        public List<LanguageDto> Languages { get; set; } = new List<LanguageDto>();

        // ========== Career Preferences (One-to-One) ==========
        public CareerPreferencesDto CareerPreferences { get; set; } = new CareerPreferencesDto();

        // ========== Volunteer & Advocacy Work ==========
        public List<VolunteerExperienceDto> VolunteerExperiences { get; set; } = new List<VolunteerExperienceDto>();

    }

    // ========== Work Experience ==========
    public class WorkExperienceDto
    {
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string EmploymentType { get; set; } // Full-time, Part-time, Contract, etc.
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string KeyResponsibilities { get; set; }
        public string LeadershipImpact { get; set; }
    }

    // ========== Education ==========
    public class EducationDto
    {
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public string InstitutionName { get; set; }
        public int? YearOfCompletion { get; set; }
    }

    // ========== Certifications ==========
    public class CertificationDto
    {
        public string CertificationName { get; set; }
        public string IssuingOrganization { get; set; }
        public int? YearEarned { get; set; }
    }

    // ========== Skills ==========
    public class SkillDto
    {
        public string SkillName { get; set; }
        public string[] SkillNames { get; set; }
        public short SkillType { get; set; } // Core, Technical, Soft
    }

    // ========== Languages ==========
    public class LanguageDto
    {
        public string Language { get; set; }
        public short Proficiency { get; set; } // Basic, Intermediate, Fluent, Native
    }

    // ========== Career Preferences ==========
    public class CareerPreferencesDto
    {
        public string PreferredJobRole { get; set; }
        public string IndustriesOfInterest { get; set; }
        public string EmploymentTypePreference { get; set; }
        public string LeadershipAspirations { get; set; }
        public string? PreferredJobLocation { get; set; }
        public string? WillingToRelocate { get; set; } // New Field

        public short? WorkMode { get; set; }

        public List<int>? PreferredSector { get; set; }

        public List<int>? FunctionalArea { get; set; }
    }

    // ========== Volunteer & Advocacy Experience ==========
    public class VolunteerExperienceDto
    {
        public string OrganizationName { get; set; }
        public string Role { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Description { get; set; }
    }
}
