using Microsoft.AspNetCore.Builder;

namespace MenuSystemWithIdentity_Core.Middleware
{
    public static class IdentityMiddlewareExtensions
    {
        public static IApplicationBuilder SeedIdentityData(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedData>();
        }
    }
}