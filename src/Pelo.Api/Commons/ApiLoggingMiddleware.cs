using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Pelo.Api.Services.LogServices;
using Pelo.Common.Models;

namespace Pelo.Api.Commons
{
    public class ApiLoggingMiddleware
    {
        readonly RequestDelegate _next;

        private ILogService _logService;

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,
                                 ILogService logService)
        {
            try
            {
                _logService = logService;

                var request = httpContext.Request;
                if (request.Path.StartsWithSegments(new PathString("/api")))
                {
                    var stopWatch = Stopwatch.StartNew();
                    var requestTime = DateTime.UtcNow;
                    var requestBodyContent = await ReadRequestBody(request);
                    var originalBodyStream = httpContext.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        var response = httpContext.Response;
                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);
                        await responseBody.CopyToAsync(originalBodyStream);

                        SafeLog(requestTime,
                                stopWatch.ElapsedMilliseconds,
                                response.StatusCode,
                                request.Method,
                                request.Path,
                                request.QueryString.ToString(),
                                requestBodyContent,
                                responseBodyContent);
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception ex)
            {
                await _next(httpContext);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            try
            {
                response.Body.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception exception)
            {
                //
            }

            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private void SafeLog(DateTime requestTime,
                             long responseMillis,
                             int statusCode,
                             string method,
                             string path,
                             string queryString,
                             string requestBody,
                             string responseBody)
        {
            if (requestBody.Length >= 10000)
            {
                _logService.WriteLog(new LogModel
                {
                    RequestTime = requestTime,
                    ResponseMillis = responseMillis,
                    StatusCode = statusCode,
                    Method = method,
                    Path = path,
                    QueryString = queryString,
                    RequestBody = string.Empty,
                    ResponseBody = responseBody
                });
            }
            else
            {
                _logService.WriteLog(new LogModel
                {
                    RequestTime = requestTime,
                    ResponseMillis = responseMillis,
                    StatusCode = statusCode,
                    Method = method,
                    Path = path,
                    QueryString = queryString,
                    RequestBody = requestBody,
                    ResponseBody = responseBody
                });
            }
        }
    }
}