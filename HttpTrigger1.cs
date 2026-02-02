using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class HttpTrigger1
{
    private readonly ILogger<HttpTrigger1> _logger;

    public HttpTrigger1(ILogger<HttpTrigger1> logger)
    {
        _logger = logger;
    }

    [Function("HttpTrigger1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        
        // Parse divisor from query string, default to 1 to avoid divide by zero
        string? divisorParam = req.Query["divisor"];
        int dividend = 10;
        int divisor = 1; // Safe default value
        
        if (!string.IsNullOrEmpty(divisorParam) && int.TryParse(divisorParam, out int parsedDivisor))
        {
            // Guard against divide by zero
            if (parsedDivisor == 0)
            {
                _logger.LogWarning("Division by zero attempted, returning error response.");
                return new BadRequestObjectResult("Error: Division by zero is not allowed.");
            }
            divisor = parsedDivisor;
        }
        
        int result = dividend / divisor;
        
        return new OkObjectResult($"Welcome to Azure Functions! Result: {result}");
    }
}