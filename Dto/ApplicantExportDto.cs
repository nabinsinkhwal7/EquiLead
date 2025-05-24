namespace EquidCMS.Dto
{
    public class ApplicantExportDto
    {
        //public int ApplicantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Pronouns { get; set; }
        public string Gender { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public string LinkedInProfile { get; set; }
        public string PersonalWebsite { get; set; }
        public string ReferenceCode { get; set; }
        public string CreatedAt { get; set; }
        //public string UpdatedAt { get; set; }
        public string YearsOfExperience { get; set; }

        // Profile Information
        public string Headline { get; set; }
        public string JobSearchStatus { get; set; }
        public string DesiredSalaryRange { get; set; }
        public string ShortBio { get; set; }
        public string InternationalExperience { get; set; }
        public string CommunityAdvocacyExperience { get; set; }

        // Career Preferences
        public string PreferredJobRole { get; set; }
        public string IndustriesOfInterest { get; set; }
        public string LeadershipAspirations { get; set; }
        public string EmploymentTypePreference { get; set; }
        public string PreferredJobLocation { get; set; }
        public string WillingToRelocate { get; set; }

        // Experience & Education
        public string WorkExperiences { get; set; }
        public string Educations { get; set; }
        public string Certifications { get; set; }
        public string VolunteerExperiences { get; set; }

        // Skills & Languages
        public string Skills { get; set; }
        public string Languages { get; set; }

        // System
        public string IsMigrated { get; set; }

    }
}
