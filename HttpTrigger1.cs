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
        
        try
        {
            // Example calculation with defensive checks
            int dividend = 10;
            int divisor = 0;
            
            if (divisor == 0)
            {
                _logger.LogWarning("Division by zero attempted. Divisor was zero.");
                return new BadRequestObjectResult("Invalid operation: Division by zero is not allowed.");
            }
            
            int result = dividend / divisor;
            return new OkObjectResult($"Welcome to Azure Functions! Result: {result}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in HttpTrigger1");
            return new ObjectResult("An unexpected error occurred.")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}