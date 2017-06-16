using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception e)
            {
                var response = new
                {
                    message = e.Message
                };
                var responseJson = JsonConvert.SerializeObject(response);

                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(responseJson);
            }
        }
    }
}
