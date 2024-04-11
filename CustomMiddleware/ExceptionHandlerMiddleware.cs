namespace AutoMed_Backend.CustomMiddleware
{
    public class ErrorDetails
    {
        public int ErrorCode { get; set; }
        public string? Message { get; set; }
    }

    public class ExceptionHandlerMiddleware
    {

        RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                string errorMessage = ex.Message;

                ErrorDetails errorDetails = new ErrorDetails()
                {
                    ErrorCode = context.Response.StatusCode,
                    Message = errorMessage
                };

                await context.Response.WriteAsJsonAsync(errorDetails);
            }
        }
    }

    public static class ErrorMiddlewareExtensions
    {
        public static void UseCustomException(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
