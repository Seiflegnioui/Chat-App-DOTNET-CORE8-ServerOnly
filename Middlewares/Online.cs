using System.Security.Claims;
using p4.Services;

namespace p4.Middlewares
{
    public class Online
    {
        private readonly ILogger<Online> _logger;
        private readonly RequestDelegate _next;

        public Online(ILogger<Online> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext ctx)
        {
            var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var id))
            {
                var userService = ctx.RequestServices.GetRequiredService<IUserService>();

                await userService.SetOnlineTimeAsync(id);
                _logger.LogInformation("Updated online time for user {UserId}", id);
            }

            await _next(ctx);
        }
    }

    public static class OnlineExtension
    {
        public static IApplicationBuilder UseOnline(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Online>();
        }
    }
}
