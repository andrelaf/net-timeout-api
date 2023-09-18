public class TimeoutMiddleware
{
  private readonly RequestDelegate _next;
  private readonly TimeSpan _timeout;

  public TimeoutMiddleware(RequestDelegate next, TimeSpan timeout)
  {
    _next = next;
    _timeout = timeout;
  }

  public async Task InvokeAsync(HttpContext context)
  {

    var logger =  context.RequestServices.GetRequiredService<ILogger<TimeoutMiddleware>>();
    using var cts = new CancellationTokenSource(_timeout);
    var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted, cts.Token);
    context.RequestAborted = linkedCts.Token;

    try
    {
      await _next(context);
    }
    catch (OperationCanceledException ex) when (context.RequestAborted.IsCancellationRequested)
    {
      logger.LogError(ex, "Timeout after {timeout}", _timeout);
      // Log ou trate o timeout aqui
      context.Response.StatusCode = 408; // Request Timeout
      await context.Response.WriteAsync("Request timed out.");
    }
  }
}

// Método de extensão para registrar o middleware
public static class TimeoutMiddlewareExtensions
{
  public static IApplicationBuilder UseTimeout(this IApplicationBuilder builder, TimeSpan timeout)
  {
    return builder.UseMiddleware<TimeoutMiddleware>(timeout);
  }
}