using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        //your http-request body parameters should contain username and password to successfully bind to api endpoint function parameter
        [Required]           //APIController attribute will make sure your data pass these validations
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}