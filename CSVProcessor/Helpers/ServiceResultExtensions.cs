using CSVProcessor.Enum;
using CSVProcessor.Models;
using Microsoft.AspNetCore.Mvc;

namespace CSVProcessor.Helpers;

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult<T>(this ServiceResult<T> result)
    {
        if (result.Success)
            return new OkResult(); 

        return result.ErrorCode switch
        {
            DataServiceErrorCode.DuplicateId => new ConflictObjectResult(result.Error),
            DataServiceErrorCode.SaveFailed => new ObjectResult(result.Error) { StatusCode = 500 },
            DataServiceErrorCode.NotFound => new NotFoundObjectResult(result.Error),
            _ => new BadRequestObjectResult(result.Error ?? "Unknown error")
        };
    }
    public static IActionResult ToActionResultWithData<T>(this ServiceResult<T> result)
    {
        if (result.Success)
            return new OkObjectResult(result.Data);

        return result.ErrorCode switch
        {
            DataServiceErrorCode.DuplicateId => new ConflictObjectResult(result.Error),
            DataServiceErrorCode.SaveFailed => new ObjectResult(result.Error) { StatusCode = 500 },
            DataServiceErrorCode.NotFound => new NotFoundObjectResult(result.Error),
            _ => new BadRequestObjectResult(result.Error ?? "Unknown error")
        };
    }
}