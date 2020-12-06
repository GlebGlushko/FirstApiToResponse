using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Weather.Logging;

namespace Weather.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FileLogger _logger;
        public RequestLoggingMiddleware(RequestDelegate next, FileLogger logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {

            Log log = new Log
            {
                Method = context.Request?.Method,
                Path = context.Request?.Path.Value,
                QueryString = context.Request.QueryString.Value,
            };
            if (log.Path != "/weather/coords" || log.Path != "/weather/city") //this filter just to save requests only from weather info(no swagger or other requests)
            {                                                                 //in real world I would log all requests, and I would implement ILogger interface and inject it
                await _next.Invoke(context);                                  //so it can be accessible from ILoggerFactory, but I decided to make it easier as the task requires 
                return;
            }
            if (context.Request.Method == "POST")
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;
                log.Payload = body;
            }
            log.RequestedTime = DateTime.Now;


            using (Stream originalRequest = context.Response.Body) //we can't read from ResponseBody, so we save reference to it and pass our stream where we can read
            {
                try
                {
                    using (var memStream = new MemoryStream())
                    {
                        context.Response.Body = memStream;
                        await _next.Invoke(context);              //invoke next middleware, after all of it our response should have some info

                        memStream.Position = 0;
                        var resp = await new StreamReader(memStream).ReadToEndAsync();
                        log.Response = resp;
                        log.ResponseCode = context.Response.StatusCode;
                        log.RespondedTime = DateTime.Now;

                        _logger.Log(log.ToString());

                        memStream.Position = 0;
                        await memStream.CopyToAsync(originalRequest);      //copy response to actual response stream
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(ex.Message);
                }
                finally
                {
                    context.Response.Body = originalRequest;
                }
            }

        }
    }
}