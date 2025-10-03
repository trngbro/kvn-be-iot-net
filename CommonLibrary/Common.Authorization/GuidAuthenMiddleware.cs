using Microsoft.AspNetCore.Http;

namespace Common.Authorization
{
    public class GuidAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public GuidAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("API-KEY", out var guidHeader) || string.IsNullOrEmpty(guidHeader))
            {
                context.Items["API-KEY"] = null;
            }

            string guid = guidHeader.ToString();

            if (!Guid.TryParse(guid, out _))
            {
                context.Items["API-KEY"] = null;
            }

            context.Items["API-KEY"] = guid;

            await _next(context);
        }
    }
}
