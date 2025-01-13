using CBT.API.Model;

namespace CBT.API.Auth
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
        Task<IEnumerable<UserManagement>> GetAll();       
        Task<UserManagement?> GetById(int id);
        Task<UserManagement?> AddAndUpdateUser(UserManagement userObj);
    }
}
