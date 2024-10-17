namespace SampleMiddlewareProject.Middlewares
{
    public class JsonRequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JsonRequestResponseLoggingMiddleware> _logger;

        public JsonRequestResponseLoggingMiddleware(RequestDelegate next, ILogger<JsonRequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            _logger.LogInformation($"Incoming Request: {requestBody}");
            context.Request.Body.Position = 0;

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            _logger.LogInformation($"Outgoing Response: {responseBodyText}");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
