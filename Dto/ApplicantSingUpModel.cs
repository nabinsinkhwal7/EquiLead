using System.ComponentModel.DataAnnotations;

namespace EquidCMS.Dto
{
    public class ApplicantSingUpModel
    {

        // 🔹 Mandatory Fields
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }


        //[Compare("Password", ErrorMessage = "Passwords do not match")]
        //public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Country Code is required")]
        public string CountryCode { get; set; }

        // 🔹 Optional Personalization Fields
        public string? JobRole { get; set; } // Job Title / Role
        public short? Industry { get; set; } // Tech, Finance, Healthcare, etc.
        public short? ExperienceLevel { get; set; } // Entry-Level, Mid-Level, etc.
        public string? Skills { get; set; } // Stored as comma-separated values (e.g., "AI, Marketing, Product Management")
        public short? PreferredJobType { get; set; } // Full-time, Freelance, etc.

        [Url(ErrorMessage = "Invalid LinkedIn URL")]
        public string? LinkedInProfile { get; set; }

        public string? Location { get; set; } // Preferred work location

        // 🔹 Additional Enhancements
        public IFormFile? ResumeFilePath { get; set; } // Store file path or URL after upload
        public short? ReferralSource { get; set; } // How did they hear about us?
        public bool ReceiveJobAlerts { get; set; } = false; // Checkbox for email notifications

    }

    public class ApplicantRegistrationModel
    {

        // 🔹 Mandatory Fields
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }


        public string Pronouns { get; set; }
        public string Gender { get; set; }
        //[Compare("Password", ErrorMessage = "Passwords do not match")]
        //public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Country Code is required")]
        public string CountryCode { get; set; }

        // 🔹 Optional Personalization Fields
       
        [Url(ErrorMessage = "Invalid LinkedIn URL")]
        public string? LinkedInProfile { get; set; }

        public string? Location { get; set; } // Preferred work location

        public string? ReferralCode { get; set; }

    }

}
