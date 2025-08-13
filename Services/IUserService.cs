using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Services
{
    public interface IUserService
    {
        Task<List<UsersDTO>> IndexAsyn();
        Task<User> RegisterAsync(UserDTO request);
        Task<string> LoginAsync(LoginDTO request);
        Task<User> CurrentUser();
    }
}