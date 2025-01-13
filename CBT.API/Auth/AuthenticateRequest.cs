using System.ComponentModel;

namespace CBT.API.Auth
{
    public class AuthenticateRequest
    {
        [DefaultValue("admin")]
        public required string Email { get; set; }

        [DefaultValue("admin")]
        public required string Password { get; set; }
    }
}
