using System.Net;
using System.Text.Json;
using VetCRM.SharedKernel;

namespace VetCRM.Api.Middleware
{
    public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ClientNotFoundException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.NotFound,
                    "client_not_found",
                    ex.Message,
                    detail: null);
            }
            catch (DuplicatePhoneException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.Conflict,
                    "duplicate_phone",
                    ex.Message,
                    detail: null);
            }
            catch (AppointmentNotFoundException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.NotFound,
                    "appointment_not_found",
                    ex.Message,
                    detail: null);
            }
            catch (PetNotFoundException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.NotFound,
                    "pet_not_found",
                    ex.Message,
                    detail: null);
            }
            catch (MedicalRecordNotFoundException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.NotFound,
                    "medical_record_not_found",
                    ex.Message,
                    detail: null);
            }
            catch (AppointmentConflictException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.Conflict,
                    "appointment_conflict",
                    ex.Message,
                    detail: null);
            }
            catch (InvalidCredentialsException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.Unauthorized,
                    "invalid_credentials",
                    ex.Message,
                    detail: null);
            }
            catch (AccountDisabledException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.Forbidden,
                    "account_disabled",
                    ex.Message,
                    detail: null);
            }
            catch (UserNotFoundException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.NotFound,
                    "user_not_found",
                    ex.Message,
                    detail: null);
            }
            catch (DuplicateEmailException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.Conflict,
                    "duplicate_email",
                    ex.Message,
                    detail: null);
            }
            catch (DomainException ex)
            {
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.BadRequest,
                    "domain_error",
                    ex.Message,
                    detail: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblemAsync(context,
                    (int)HttpStatusCode.InternalServerError,
                    "internal_server_error",
                    "An unexpected error occurred.",
                    detail: null);
            }
        }

        private static async Task WriteProblemAsync(
            HttpContext context,
            int statusCode,
            string type,
            string title,
            string? detail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var traceId = context.TraceIdentifier ?? System.Diagnostics.Activity.Current?.Id;

            var response = new
            {
                type,
                title,
                status = statusCode,
                detail,
                traceId
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
