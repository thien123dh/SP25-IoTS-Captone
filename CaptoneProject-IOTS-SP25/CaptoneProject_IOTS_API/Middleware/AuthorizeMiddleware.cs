namespace CaptoneProject_IOTS_API.Middleware
{
    public class AuthorizeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthorizeMiddleware> _logger;

        public AuthorizeMiddleware(RequestDelegate next, ILogger<AuthorizeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Gọi middleware tiếp theo
                await _next(context);

                // Kiểm tra nếu mã trạng thái là 401 hoặc 403 sau khi xử lý
                if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
                {
                    await HandleUnauthorizedResponseAsync(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                // Xử lý lỗi với phản hồi JSON
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new
                {
                    Code = 500,
                    Message = "Internal Server Error",
                    Detailed = ex.Message,
                };

                var responseText = System.Text.Json.JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(responseText);
            }
            else
            {
                _logger.LogError("Response has already started, cannot modify it.");
            }
        }

        private async Task HandleUnauthorizedResponseAsync(HttpContext context)
        {
            if (!context.Response.HasStarted)
            {
                var statusCode = context.Response.StatusCode;
                var message = statusCode == 401 ? "Unauthorized access" : "Forbidden access";

                _logger.LogError($"Status code: {statusCode}");

                context.Response.ContentType = "application/json";

                var response = new
                {
                    Code = statusCode,
                    Message = message,
                };

                var responseText = System.Text.Json.JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(responseText);
            }
            else
            {
                _logger.LogWarning("Response has already started, cannot write 401/403 response.");
            }
        }
    }
}
