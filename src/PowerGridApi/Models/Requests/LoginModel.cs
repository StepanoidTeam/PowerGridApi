
using System.ComponentModel.DataAnnotations;

namespace PowerGridEngine
{
    public class LoginModel 
    {
        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

    }
}
