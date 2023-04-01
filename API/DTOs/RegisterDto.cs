using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        //your http-request body parameters should contain username and password to successfully bind to api endpoint function parameter
        [Required]           //APIController attribute will make sure your data pass these validations
        public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateOnly? DateOfBirth { get; set; } //optional to make required work!
        [Required] public string city { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 7)]
        public string Password { get; set; }
    }
}