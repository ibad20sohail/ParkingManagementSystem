using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ParkingManagementSystem.Filters
{
    public partial class GlobalLoggingFilter : IActionFilter
    {
        private readonly ILogger<GlobalLoggingFilter> _logger;

        public GlobalLoggingFilter(ILogger<GlobalLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;

            if ((method == "POST" || method == "PUT") && context.ActionArguments.Count > 0)
            {
                var requestModelsJson = JsonSerializer.Serialize(context.ActionArguments);
                LogMvcRequest(_logger, method, path, requestModelsJson);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var path = context.HttpContext.Request.Path;

            if (context.Result is ViewResult viewResult)
            {
                var model = viewResult.Model;
                string responseModelJson = model != null ? JsonSerializer.Serialize(model) : "[No Model]";

                LogMvcResponse(_logger, path, responseModelJson);
            }
        }

        // High-Performance Source-Generated Loggers
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "MVC Request Model: {Method} {Path} | Bound Data: {Arguments}")]
        private static partial void LogMvcRequest(ILogger logger, string method, PathString path, string arguments);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "MVC Response Model: {Path} | View Data: {Model}")]
        private static partial void LogMvcResponse(ILogger logger, PathString path, string model);
    }
}
