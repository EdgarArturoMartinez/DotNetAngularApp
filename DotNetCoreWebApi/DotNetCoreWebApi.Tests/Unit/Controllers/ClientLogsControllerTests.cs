using DotNetCoreWebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetCoreWebApi.Tests.Unit.Controllers;

/// <summary>
/// Unit tests for ClientLogsController
/// Verifies the frontend log-forwarding endpoint
/// </summary>
public class ClientLogsControllerTests
{
    private readonly Mock<ILogger<ClientLogsController>> _mockLogger;
    private readonly ClientLogsController _controller;

    public ClientLogsControllerTests()
    {
        _mockLogger = new Mock<ILogger<ClientLogsController>>();
        _controller = new ClientLogsController(_mockLogger.Object);
    }

    #region Post Tests

    [Fact]
    public void Post_WithValidEntries_ReturnsAccepted()
    {
        // Arrange
        var entries = new[]
        {
            new ClientLogEntry { Level = "info", Message = "Page loaded", Source = "AppComponent", Timestamp = "2026-03-02T10:00:00Z" },
            new ClientLogEntry { Level = "error", Message = "API call failed", Source = "AuthService", Timestamp = "2026-03-02T10:00:01Z" }
        };

        // Act
        var result = _controller.Post(entries);

        // Assert
        result.Should().BeOfType<AcceptedResult>();
    }

    [Fact]
    public void Post_WithNullEntries_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Post(null!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Post_WithEmptyArray_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Post(Array.Empty<ClientLogEntry>());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData("info")]
    [InlineData("warn")]
    [InlineData("warning")]
    [InlineData("error")]
    [InlineData("fatal")]
    [InlineData("debug")]
    [InlineData("trace")]
    [InlineData("unknown")]
    public void Post_WithDifferentLogLevels_ReturnsAccepted(string level)
    {
        // Arrange
        var entries = new[]
        {
            new ClientLogEntry { Level = level, Message = "Test message", Source = "TestComponent", Timestamp = "2026-03-02T10:00:00Z" }
        };

        // Act
        var result = _controller.Post(entries);

        // Assert
        result.Should().BeOfType<AcceptedResult>();
    }

    [Fact]
    public void Post_WithNullFieldsInEntry_ReturnsAccepted()
    {
        // Arrange - entries with null fields should still be processed gracefully
        var entries = new[]
        {
            new ClientLogEntry { Level = null, Message = null, Source = null, Timestamp = null }
        };

        // Act
        var result = _controller.Post(entries);

        // Assert
        result.Should().BeOfType<AcceptedResult>();
    }

    [Fact]
    public void Post_WithMultipleEntries_ProcessesAll()
    {
        // Arrange
        var entries = new[]
        {
            new ClientLogEntry { Level = "info", Message = "First", Source = "A" },
            new ClientLogEntry { Level = "warn", Message = "Second", Source = "B" },
            new ClientLogEntry { Level = "error", Message = "Third", Source = "C" },
            new ClientLogEntry { Level = "debug", Message = "Fourth", Source = "D" },
            new ClientLogEntry { Level = "fatal", Message = "Fifth", Source = "E" }
        };

        // Act
        var result = _controller.Post(entries);

        // Assert
        result.Should().BeOfType<AcceptedResult>();
    }

    [Fact]
    public void Post_WithStackTrace_ReturnsAccepted()
    {
        // Arrange
        var entries = new[]
        {
            new ClientLogEntry
            {
                Level = "error",
                Message = "Unhandled exception",
                Source = "GlobalErrorHandler",
                Timestamp = "2026-03-02T10:00:00Z",
                StackTrace = "Error: Unhandled exception\n    at AppComponent.ngOnInit (app.ts:25)"
            }
        };

        // Act
        var result = _controller.Post(entries);

        // Assert
        result.Should().BeOfType<AcceptedResult>();
    }

    #endregion
}
