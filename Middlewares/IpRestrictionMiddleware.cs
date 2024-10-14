namespace SampleMiddlewareProject.Middlewares
{
    public class IpRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IpRestrictionMiddleware> _logger;
        private readonly IConfiguration _config;

        public IpRestrictionMiddleware(RequestDelegate next, ILogger<IpRestrictionMiddleware> logger, IConfiguration config)
        {
            _next = next;
            _logger = logger;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestIp = context.Connection.RemoteIpAddress?.ToString();

            string[] myArray = _config.GetSection("WhiteIpAddresses").Get<string[]>()!;

            if (!myArray.Contains(requestIp))
            {
                _logger.LogWarning($"Unauthorized request from IP: {requestIp}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied.");
                return;
            }

            await _next(context);
        }
    }
}
