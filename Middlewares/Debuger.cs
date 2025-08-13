using System.Security.Claims;

namespace p4.Middlewares
{
    public class Debuger(ILogger<Debuger> logger, RequestDelegate next)
    {
        public async Task Invoke(HttpContext ctx)
        {
            var identity = ctx.User?.Identity;
            var isAuth = identity?.IsAuthenticated ?? false;

            logger.LogInformation("Authenticated: {auth}, Name: {name}, Claims: {claims}", 
                isAuth,
                identity?.Name ?? "anonymous",
                string.Join(", ", ctx.User.Claims.Select(c => $"{c.Type} = {c.Value}"))
            );

            await next(ctx);
        }
    }

    public static class DebugerExtension
    {
        public static IApplicationBuilder UseCustomDebuger(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Debuger>();
        }
    }
}
