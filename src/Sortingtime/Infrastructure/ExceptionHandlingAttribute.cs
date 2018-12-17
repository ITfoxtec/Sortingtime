using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sortingtime.Infrastructure
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {        
        private readonly Type type;

        public ExceptionHandlingAttribute(Type type)
        {
            this.type = type;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception != null)
            {
                var logger = CreateLogger(context.HttpContext);
                if(type == Type.Page)
                {
                    logger.LogError(context.Exception, context.Exception?.Message);
                }
                else
                {
                    logger.LogError(context.Exception, GetPartitionAndUserContextToString(context.HttpContext));
                }
            }
            SetErrorResponse(context);

            base.OnException(context);
        }

        private ILogger CreateLogger(HttpContext httpContext)
        {
            return httpContext.RequestServices.GetService<ILogger<ExceptionHandlingAttribute>>();
        }

        private void SetErrorResponse(ExceptionContext context)
        {
            if (type == Type.Page)
            {
                context.Result = new RedirectToActionResult("Error", "E", null);
            }
            else if (type == Type.SecurePage)
            {
                context.Result = new RedirectToActionResult("Error", "A", null);
            }
            else
            {
                if (context.Exception is NotFoundResultException)
                {
                    context.Result = new NotFoundResult();
                }
                else
                {
                    context.Result = new JsonResult(new
                    {
                        Message = string.Format("[{0}] {1}", GetUserContextToString(context.HttpContext), context.Exception?.Message),
                        StackTrace = context.Exception?.StackTrace
                    })
                    {
                        StatusCode = 500
                    };
                }
            }
        }

        private string GetUserContextToString(HttpContext httpContext)
        {
            try
            {
                return string.Format("UserId: {0}", httpContext.User.GetUserId());
            }
            catch (Exception exc)
            {
                var logger = CreateLogger(httpContext);
                logger.LogError(exc, "Unable to read user context.");
                return string.Empty;
            }
        }

        private string GetPartitionAndUserContextToString(HttpContext httpContext)
        {
            try
            {
                return string.Format("PartitionId: {0}, UserId: {1}", httpContext.User.GetPartitionId(), httpContext.User.GetUserId());
            }
            catch (Exception exc)
            {
                var logger = CreateLogger(httpContext);
                logger.LogError(exc, "Unable to read user context.");
                return string.Empty;
            }
        }

        public enum Type
        {            
            Page,
            SecurePage,
            Api
        }
    }
}
