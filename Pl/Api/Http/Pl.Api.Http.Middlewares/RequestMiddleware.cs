//using System.Diagnostics;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Http;

//using Prometheus;

//namespace Api.Middlewares;

//public class RequestMiddleware
//{
//    private readonly RequestDelegate _next;

//    private static readonly Histogram ResponseTimeHistogram = Metrics.CreateHistogram(
//        "http_request_duration_seconds",
//        "Duration of HTTP requests in seconds",
//        new HistogramConfiguration
//        {
//            LabelNames = new[] { "method", "endpoint", "status_code" },
//            Buckets = Histogram.ExponentialBuckets(0.01, 2, 10)
//        });

//    private static readonly Counter ErrorCounter = Metrics.CreateCounter(
//        "http_requests_errors_total",
//        "Total number of HTTP errors",
//        new CounterConfiguration
//        {
//            LabelNames = new[] { "method", "endpoint", "status_code" }
//        });

//    public RequestMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        var stopwatch = Stopwatch.StartNew();
//        var endpoint = context.Request.Path;
//        var method = context.Request.Method;

//        try
//        {
//            await _next(context);
//        }
//        catch
//        {
//            context.Response.StatusCode = 500;
//            throw;
//        }
//        finally
//        {
//            var statusCode = context.Response.StatusCode;
//            var statusCodeStr = statusCode.ToString();

//            ResponseTimeHistogram
//                .Labels(method, endpoint, statusCodeStr)
//                .Observe(stopwatch.Elapsed.TotalSeconds);

//            if (statusCodeStr.StartsWith('4') || statusCodeStr.StartsWith('5'))
//            {
//                ErrorCounter.Labels(method, endpoint, statusCodeStr).Inc();
//            }
//        }
//    }
//}