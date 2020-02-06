using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YMNNetCoreFrameWork.Host.Middles
{
    public static class ExceptionHandlingExtensions
    {
        public static void UseMyExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(builder => {

                builder.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        //记录日志
                        var logger = loggerFactory.CreateLogger("BlogDemo.Api.Extensions.ExceptionHandlingExtensions");
                        logger.LogDebug(500, ex.Error, ex.Error.Message);
                    }
                    await context.Response.WriteAsync(ex?.Error?.Message ?? "an error occure");
                });
            });
        }
    }
}
