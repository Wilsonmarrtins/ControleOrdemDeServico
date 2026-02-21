using System.Net;
using System.Text.Json;

namespace OsService.ApiService;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ArgumentException ex)
        {
            await Write(ctx, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await Write(ctx, HttpStatusCode.NotFound, ex.Message);
        }
    }

    private static Task Write(HttpContext ctx, HttpStatusCode status, string message)
    {
        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json; charset=utf-8";

        var payload = JsonSerializer.Serialize(new { error = message });
        return ctx.Response.WriteAsync(payload);
    }
}