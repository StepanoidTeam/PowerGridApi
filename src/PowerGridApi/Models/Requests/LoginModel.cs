
using System.ComponentModel.DataAnnotations;

namespace PowerGridEngine
{
    public class LoginModel : IWebSocketRequestModel
    {
        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

    }
}
