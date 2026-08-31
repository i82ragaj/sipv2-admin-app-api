using Microsoft.AspNetCore.Diagnostics;

namespace SIPV2.AdminAppApi.Services;

// Único punto donde una excepción no controlada llega a loguearse con traza
// completa (via Serilog, consola + fichero) antes de devolver al cliente un
// 500 genérico -- nunca el mensaje/stack trace real de la excepción.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Excepción no controlada en {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(
            new { message = "Ha ocurrido un error inesperado." },
            cancellationToken);

        return true;
    }
}
