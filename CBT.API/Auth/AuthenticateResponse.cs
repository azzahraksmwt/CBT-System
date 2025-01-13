using CBT.API.Model;

namespace CBT.API.Auth
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(UserManagement user, string token)
        {
            Id = user.id;
            Name = user.name;
            Email = user.email;
            Token = token;
        }
    }
}
