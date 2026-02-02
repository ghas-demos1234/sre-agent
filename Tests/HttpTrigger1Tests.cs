using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Company.Function;

namespace sreagnet_func.Tests;

public class HttpTrigger1Tests
{
    private readonly Mock<ILogger<HttpTrigger1>> _mockLogger;
    private readonly HttpTrigger1 _function;

    public HttpTrigger1Tests()
    {
        _mockLogger = new Mock<ILogger<HttpTrigger1>>();
        _function = new HttpTrigger1(_mockLogger.Object);
    }

    [Fact]
    public void Run_WithoutDivisorParameter_ReturnsSuccessWithDefaultResult()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;

        // Act
        var result = _function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Welcome to Azure Functions! Result: 10", okResult.Value);
    }

    [Fact]
    public void Run_WithValidDivisor_ReturnsCorrectCalculation()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?divisor=2");
        var request = context.Request;

        // Act
        var result = _function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Welcome to Azure Functions! Result: 5", okResult.Value);
    }

    [Fact]
    public void Run_WithZeroDivisor_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?divisor=0");
        var request = context.Request;

        // Act
        var result = _function.Run(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error: Division by zero is not allowed.", badRequestResult.Value);
    }

    [Fact]
    public void Run_WithInvalidDivisor_ReturnsSuccessWithDefaultResult()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?divisor=invalid");
        var request = context.Request;

        // Act
        var result = _function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Welcome to Azure Functions! Result: 10", okResult.Value);
    }

    [Fact]
    public void Run_WithEmptyDivisor_ReturnsSuccessWithDefaultResult()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?divisor=");
        var request = context.Request;

        // Act
        var result = _function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Welcome to Azure Functions! Result: 10", okResult.Value);
    }

    [Fact]
    public void Run_WithNegativeDivisor_ReturnsCorrectCalculation()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?divisor=-5");
        var request = context.Request;

        // Act
        var result = _function.Run(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Welcome to Azure Functions! Result: -2", okResult.Value);
    }

    [Fact]
    public void Run_LogsInformation_OnEveryCall()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;

        // Act
        _function.Run(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("C# HTTP trigger function processed a request")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Run_LogsWarning_WhenZeroDivisorAttempted()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?divisor=0");
        var request = context.Request;

        // Act
        _function.Run(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Division by zero attempted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
