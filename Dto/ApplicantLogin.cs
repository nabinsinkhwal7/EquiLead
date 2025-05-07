using System.ComponentModel.DataAnnotations;

namespace EquidCMS.Dto
{
    public class ApplicantLogin
    {
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
