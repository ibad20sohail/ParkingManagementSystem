using Microsoft.IO;
using System.Diagnostics;
using System.Text;

namespace ParkingManagmentSystem.Middlewares
{
    public partial class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private static readonly RecyclableMemoryStreamManager StreamManager = new();

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path != null && (
                path.Contains('.') ||
                path.StartsWith("/css", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/js", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            var request = context.Request;
            var stopwatch = Stopwatch.StartNew();

            string requestBodyText = await ReadRequestBodyAsync(request);

            LogFastRequest(_logger, request.Method, request.Path, string.IsNullOrWhiteSpace(requestBodyText) ? "[Empty]" : requestBodyText);

            var originalResponseBodyStream = context.Response.Body;
            using var memoryResponseBodyStream = StreamManager.GetStream();
            context.Response.Body = memoryResponseBodyStream;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var response = context.Response;

                string responseBodyText = await ReadResponseBodyAsync(memoryResponseBodyStream);

                LogFastResponse(_logger, request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds, string.IsNullOrWhiteSpace(responseBodyText) ? "[Empty]" : responseBodyText);

                await memoryResponseBodyStream.CopyToAsync(originalResponseBodyStream);
            }
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            if (request.ContentLength == null || request.ContentLength == 0) return "[Empty]";

            request.EnableBuffering();

            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            request.Body.Position = 0;
            return body;
        }

        private static async Task<string> ReadResponseBodyAsync(MemoryStream memoryStream)
        {
            if (memoryStream.Length == 0) return "[Empty]";

            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            memoryStream.Position = 0;
            return body;
        }

        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "HTTP Request: {Method} {Path} | Body: {Body}")]
        private static partial void LogFastRequest(ILogger logger, string method, PathString path, string body);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "HTTP Response: {Method} {Path} answered {StatusCode} in {ElapsedMs}ms | Body: {Body}")]
        private static partial void LogFastResponse(ILogger logger, string method, PathString path, int statusCode, long elapsedMs, string body);
    }
}
