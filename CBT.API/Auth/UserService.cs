using CBT.API.DbContext;
using CBT.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBT.API.Auth
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly CBTContext db;

        public UserService(IOptions<AppSettings> appSettings, CBTContext _db)
        {
            _appSettings = appSettings.Value;
            db = _db;
        }

        public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
        {
            var user = await db.UserManagement
                .FirstOrDefaultAsync(x => x.email == model.Email && x.password == model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = await generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public async Task<IEnumerable<UserManagement>> GetAll()
        {
            return await db.UserManagement.ToListAsync();
        }

        public async Task<UserManagement?> GetById(int id)
        {
            return await db.UserManagement.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<UserManagement?> AddAndUpdateUser(UserManagement userObj)
        {
            bool isSuccess = false;
            if (userObj.id > 0)
            {
                var obj = await db.UserManagement.FirstOrDefaultAsync(c => c.id == userObj.id);
                if (obj != null)
                {
                    db.UserManagement.Update(obj);
                    isSuccess = await db.SaveChangesAsync() > 0;
                }
            }
            else
            {
                await db.UserManagement.AddAsync(userObj);
                isSuccess = await db.SaveChangesAsync() > 0;
            }

            return isSuccess ? userObj : null;
        }
        // helper methods
        private async Task<string> generateJwtToken(UserManagement user)
        {
            // Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await Task.Run(() =>
            {
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("id", user.id.ToString()), // Existing id claim
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()), // Add the sub claim
                new Claim(JwtRegisteredClaimNames.Email, user.email), // Optionally add other claims
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Add a unique identifier for the token
            }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                return tokenHandler.CreateToken(tokenDescriptor);
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
