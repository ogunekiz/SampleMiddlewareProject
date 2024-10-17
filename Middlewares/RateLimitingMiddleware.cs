namespace SampleMiddlewareProject.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static Dictionary<string, (DateTime requestTime, int requestCount)> _clientRequests = new();
        private readonly int _requestLimit = 5;
        private readonly TimeSpan _timeSpan = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString();

            if (!_clientRequests.ContainsKey(clientIp))
            {
                _clientRequests[clientIp] = (DateTime.UtcNow, 1);
            }
            else
            {
                var (lastRequestTime, requestCount) = _clientRequests[clientIp];
                var timeDifference = DateTime.UtcNow - lastRequestTime;

                if (timeDifference <= _timeSpan && requestCount >= _requestLimit)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                    return;
                }

                if (timeDifference > _timeSpan)
                {
                    _clientRequests[clientIp] = (DateTime.UtcNow, 1);
                }
                else
                {
                    _clientRequests[clientIp] = (lastRequestTime, requestCount + 1);
                }
            }

            await _next(context);
        }
    }
}
