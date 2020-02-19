using Microsoft.AspNetCore.Builder;
using System.Net.Http;
using System.Threading.Tasks;

namespace ViennaNET.WebApi.Cors
{
  internal static class CorsConfigurator
  {
    public static void Configure(IApplicationBuilder app)
    {
      RegisterPreflightMiddleware(app);
      app.UseCors(x => x.SetIsOriginAllowed(o => true)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
    }

    private static void RegisterPreflightMiddleware(IApplicationBuilder app)
    {
      app.Use((context, next) =>
      {
        if (context.Request.Method == HttpMethod.Options.Method)
        {
          context.Request.Headers.TryGetValue("Origin", out var origin);
          context.Response.Headers.Add("Access-Control-Allow-Headers", "content-type,x-request-id,x-user-id,x-user-domain,cache-control,authorization");
          context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
          context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,PATCH,OPTIONS");
          context.Response.Headers.Add("Access-Control-Allow-Origin", origin);

          return Task.CompletedTask;
        }

        return next();
      });
    }
  }
}
