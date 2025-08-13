using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using p4.Data;
using p4.Models.DTO;
using p4.Models.Entities;

namespace p4.Services
{
    public class UserService(IConfiguration config,ILogger<UserService> logger ,AppDbContext context, IHttpContextAccessor http) : IUserService
    {
        public async Task<User> CurrentUser()
        {
            var userClaims = http.HttpContext?.User;

            if (userClaims == null || !userClaims.Identity!.IsAuthenticated)
                return null;

            var userIdString = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return null;

            if (!int.TryParse(userIdString, out int userId))
                return null;

            var user = await context.Users.FindAsync(userId);
            return user;
        }

        public async Task<List<UsersDTO>> IndexAsyn()
        {
            var me = http.HttpContext?.User;
            var myId = int.Parse(me?.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return await context.Users.Where(u => u.Id != myId).Select(u => new UsersDTO
            {
                Id = u.Id,
                email = u.Email,
                role = u.role,
                photo = u.photo,
                username = u.username,
                last_seen = u.last_seen

            }).ToListAsync();

        }
        public async Task<string> LoginAsync(LoginDTO request)
        {

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.email);
            if (user is null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, request.password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            var token = CreateToken(user);

            return token;
        }

        public async Task<User> RegisterAsync(UserDTO request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.email))
            {
                return null;
            }
            var folder_path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folder_path))
                Directory.CreateDirectory(folder_path);

            var filename = request.username +"-" + Guid.NewGuid().ToString() + Path.GetExtension(request.photo.FileName);
            var filepath = Path.Combine(folder_path, filename);

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await request.photo.CopyToAsync(stream);
            }
            var user = new User();
            var hashed = new PasswordHasher<User>().HashPassword(user, request.password);
            user.HashedPassword = hashed;
            user.Email = request.email;
            user.photo = filename;
            user.username = request.username;
            user.last_seen = request.last_seen;

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register user", ex);

            }

            return user;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.role.ToString())
            };

            var bytekey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("jwt_infos:key")!));
            var creds = new SigningCredentials(bytekey, SecurityAlgorithms.HmacSha512);

            var descriptor = new JwtSecurityToken(
                issuer: config.GetValue<string>("jwt_infos:issuer"),
                audience: config.GetValue<string>("jwt_infos:audience"),
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddDays(2),
                claims: claims

            );
            return new JwtSecurityTokenHandler().WriteToken(descriptor);
        }
    }
}