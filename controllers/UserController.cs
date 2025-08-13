using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using p4.Models.DTO;
using p4.Models.Entities;
using p4.Services;

namespace p4.controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IUserService userService, ILogger<AuthController> log) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO req)
        {
            log.LogInformation(req.username);
            var user = await userService.RegisterAsync(req);
            return user == null ? BadRequest("already exist") : Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO req)
        {
            var user = await userService.LoginAsync(req);
            return user != null ? Ok(user) : BadRequest("Password or email is wrong");
        }

        [HttpGet("index")]
        public async Task<List<UsersDTO>> index()
        {
            return await userService.IndexAsyn();
        }

        [HttpGet("current")]
        public async Task<IActionResult> CurrentUser()
        {
            var user = await userService.CurrentUser();
            return Ok(user);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminRoute()
        {
            return Ok("admin");
        }

        [HttpGet("User")]
        [Authorize(Roles = "User")]
        public IActionResult UserRoute()
        {
            return Ok("userrr");
        }

        [HttpGet("global")]
        [Authorize]
        public IActionResult Global()
        {
            return Ok("yooooooooooo");
        }
    }
}