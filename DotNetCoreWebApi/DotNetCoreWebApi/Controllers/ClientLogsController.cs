using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers;

/// <summary>
/// Receives client-side log entries from the Angular frontend.
/// Allows the frontend to forward errors, warnings, and info events
/// to the backend Serilog pipeline for unified log aggregation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClientLogsController : ControllerBase
{
    private readonly ILogger<ClientLogsController> _logger;

    public ClientLogsController(ILogger<ClientLogsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Receive a batch of log entries from the frontend client
    /// </summary>
    /// <param name="entries">Array of client log entries</param>
    /// <returns>Acceptance confirmation</returns>
    [HttpPost]
    public IActionResult Post([FromBody] ClientLogEntry[] entries)
    {
        if (entries == null || entries.Length == 0)
            return BadRequest(new { message = "No log entries provided" });

        foreach (var entry in entries)
        {
            var message = "[CLIENT] {Source} | {ClientMessage}";

            switch (entry.Level?.ToLowerInvariant())
            {
                case "error":
                case "fatal":
                    _logger.LogError(message, entry.Source ?? "Unknown", entry.Message ?? "No message");
                    break;
                case "warn":
                case "warning":
                    _logger.LogWarning(message, entry.Source ?? "Unknown", entry.Message ?? "No message");
                    break;
                case "debug":
                case "trace":
                    _logger.LogDebug(message, entry.Source ?? "Unknown", entry.Message ?? "No message");
                    break;
                default: // info
                    _logger.LogInformation(message, entry.Source ?? "Unknown", entry.Message ?? "No message");
                    break;
            }
        }

        return Accepted(new { received = entries.Length });
    }
}

/// <summary>
/// Represents a single log entry sent from the client-side application
/// </summary>
public class ClientLogEntry
{
    /// <summary>Log level: trace, debug, info, warn, error, fatal</summary>
    public string? Level { get; set; }

    /// <summary>The log message content</summary>
    public string? Message { get; set; }

    /// <summary>Source component or service that generated the log</summary>
    public string? Source { get; set; }

    /// <summary>ISO 8601 timestamp from the client</summary>
    public string? Timestamp { get; set; }

    /// <summary>Optional stack trace for errors</summary>
    public string? StackTrace { get; set; }
}
