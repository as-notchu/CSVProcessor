using CSVProcessor.Enum;
using CSVProcessor.Models;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Helpers;

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult(this ServiceResult result, ILogger logger)
    {
        if (result.Success)
        {
            return new OkResult();  
        }
        
        logger.LogError($"Error occured: {result.Error}");
        return result.ErrorCode switch
        {
            ServiceErrorCodes.Duplicate => new ConflictObjectResult(result.Error),
            ServiceErrorCodes.SaveFailed => new ObjectResult(result.Error) { StatusCode = 500 },
            ServiceErrorCodes.NotFound => new NotFoundObjectResult(result.Error),
            ServiceErrorCodes.CantParseData => new BadRequestObjectResult(result.Error) { StatusCode = 400 },
            _ => new BadRequestObjectResult(result.Error ?? "Unknown error")
        };
    }
    public static IActionResult ToActionResult<T>(this ServiceResult<T> result, ILogger logger)
    {
        if (result.Success)
        {
            return new OkResult();  
        }
        
        logger.LogError($"Error occured: {result.Error}");
        return result.ErrorCode switch
        {
            ServiceErrorCodes.Duplicate => new ConflictObjectResult(result.Error),
            ServiceErrorCodes.SaveFailed => new ObjectResult(result.Error) { StatusCode = 500 },
            ServiceErrorCodes.NotFound => new NotFoundObjectResult(result.Error),
            ServiceErrorCodes.CantParseData => new BadRequestObjectResult(result.Error) { StatusCode = 400 },
            _ => new BadRequestObjectResult(result.Error ?? "Unknown error")
        };
    }
    public static IActionResult ToActionResultWithData<T>(this ServiceResult<T> result, ILogger logger)
    {
        if (result.Success)
            return new OkObjectResult(result.Data);

        logger.LogError($"Error occured: {result.Error}");
        return result.ErrorCode switch
        {
            ServiceErrorCodes.Duplicate => new ConflictObjectResult(result.Error),
            ServiceErrorCodes.SaveFailed => new ObjectResult(result.Error) { StatusCode = 500 },
            ServiceErrorCodes.NotFound => new NotFoundObjectResult(result.Error),
            ServiceErrorCodes.CantParseData => new BadRequestObjectResult(result.Error) { StatusCode = 400 },
            _ => new BadRequestObjectResult(result.Error ?? "Unknown error")
        };
    }
}