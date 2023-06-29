using System.Globalization;
using Microsoft.AspNetCore.Builder;

namespace CaterSoft.IdentityServer
{
    public static class CultureMiddlewareExtensions
    {
        public static void ConfigureCultureHandler(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.QueryString.HasValue)
                {
                    var currentUserSlug = context.Request.QueryString.Value.Split('=')[1];
                    var defaultCulture = new CultureInfo("en-UK");
                    CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
                    CultureInfo.CurrentCulture = defaultCulture;
                    CultureInfo.CurrentUICulture = defaultCulture;
                }
                await next();
            });
        }
    }
}