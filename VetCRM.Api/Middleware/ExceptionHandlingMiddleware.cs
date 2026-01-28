using System.Net;
using System.Text.Json;
using VetCRM.SharedKernel;

namespace VetCRM.Api.Middleware
{
    public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ClientNotFoundException ex)
            {
                await WriteErrorAsync(context,
                                      HttpStatusCode.NotFound,
                                      "client_not_found",
                                      ex.Message);
            }
            catch(DomainException ex)
            {
                await WriteErrorAsync(context,
                                      HttpStatusCode.BadRequest,
                                      "domain_error",
                                      ex.Message);
            }
        }

        private async Task WriteErrorAsync(HttpContext context,
                                           HttpStatusCode statusCode,
                                           string type,
                                           string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                type,
                title = message,
                status = (int)statusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
