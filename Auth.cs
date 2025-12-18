using Microsoft.AspNetCore.Http;
using server.Users.Repositories;

namespace server;

public static class Auth
{
    public static bool HasValidSession(this HttpContext ctx)
    {
        return ctx.Session.IsAvailable && ctx.Session.GetInt32("user_id").HasValue;
    }

    public static async Task<bool> IsAdminAsync(this HttpContext ctx, Config config)
    {
        if (!ctx.HasValidSession())
        {
            return false;
        }

        var userRepo = new UserRepository(config);
        return await userRepo.GetAdminStatus(ctx);
    }
}
