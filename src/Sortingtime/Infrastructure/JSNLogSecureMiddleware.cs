//using System.Threading.Tasks;
//using JSNLog.Infrastructure;
//using System.IO;
//using System.Text;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Text.RegularExpressions;
//using Microsoft.AspNetCore.Http;

//namespace Sortingtime.Infrastructure
//{
//    // Kopieret fra https://github.com/mperdeck/jsnlog/blob/aa3b186a36de49e930459898549afa225af7962c/src/JSNLog/PublicFacing/AspNet5/LogRequestHandling/Middleware/JSNLogMiddleware.cs
//    public class JSNLogSecureMiddleware
//    {
//        private static Regex _regex = new Regex(@";\s*charset=(?<charset>[^\s;]+)");
//        private readonly RequestDelegate next;

//        public JSNLogSecureMiddleware(RequestDelegate next) 
//        {
//            this.next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            if (context.User.Identity.IsAuthenticated)
//            {
//                string url = context.Request.GetDisplayUrl();
//                if (LoggingUrlHelpers.IsLoggingUrl(url))
//                {
//                    var encoding = GetEncoding(context);

//                    MemoryStream destination = new MemoryStream();
//                    var writer = new StreamWriter(destination, encoding);

//                    using (var reader = new StreamReader(context.Request.Body, encoding))
//                    {
//                        var json = reader.ReadToEnd();
//                        json = json.Replace("\"m\":\"{\\\"e\\\":{", "\"m\":\"{\\\"e\\\":{\\\"serverMessage\\\":\\\"" + GetPartitionAndUserContextToString(context) + "\\\", ");
//                        writer.Write(json);
//                    }

//                    writer.Flush();

//                    destination.Seek(0, SeekOrigin.Begin);
//                    context.Request.Body = destination;

//                    var jsNLogMiddleware = new JSNLogMiddleware(next);
//                    await jsNLogMiddleware.Invoke(context);
//                    return;
//                }
//            }

//            await next(context);            
//        }

//        // fra https://github.com/mperdeck/jsnlog/blob/aa3b186a36de49e930459898549afa225af7962c/src/JSNLog/Infrastructure/HttpHelpers.cs
//        private static Encoding GetEncoding(HttpContext context)
//        {
//            var contentType = context.Request.Headers["Content-Type"].ToString();
//            if (string.IsNullOrEmpty(contentType))
//            {
//                return Encoding.UTF8;
//            }

//            string charset = "utf-8";
//            var match = _regex.Match(contentType);
//            if (match.Success)
//            {
//                charset = match.Groups["charset"].Value;
//            }

//            try
//            {
//                return Encoding.GetEncoding(charset);
//            }
//            catch
//            {
//                return Encoding.UTF8;
//            }
//        }

//        private string GetPartitionAndUserContextToString(HttpContext httpContext)
//        {
//            try
//            {
//                return string.Format("PartitionId: {0}, UserId: {1}", httpContext.User.GetPartitionId(), httpContext.User.GetUserId());
//            }
//            catch (Exception exc)
//            {
//                var logger = CreateLogger(httpContext);
//                logger.LogError(exc, "Unable to read user context.");
//                return string.Empty;
//            }
//        }

//        private ILogger CreateLogger(HttpContext httpContext)
//        {
//            return httpContext.ApplicationServices.GetService<ILoggerFactory>().CreateLogger<JSNLogSecureMiddleware>();
//        }
//    }
//}
