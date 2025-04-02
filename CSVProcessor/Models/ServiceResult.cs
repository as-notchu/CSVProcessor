using CSVProcessor.Enum;
using CSVProcessor.Models.DTO;

namespace CSVProcessor.Models;

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }
    
    public List<WarningsDetails> WarningsDetails { get; set; }

    public static ServiceResult<T> Ok(T data, List<WarningsDetails>? warningsDetailsList = null) => new()
    {
        Success = true,
        Data = data,
        WarningsDetails = warningsDetailsList ?? new List<WarningsDetails>()
    };

    public static ServiceResult<T> Fail(ServiceErrorCodes codes, string message) => new()
    {
        Success = false,
        ErrorCode = codes,
        Error = message
    };
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ServiceErrorCodes ErrorCode { get; set; } = ServiceErrorCodes.None;

    public static ServiceResult Ok() => new() { Success = true };

    public static ServiceResult Fail(ServiceErrorCodes code, string message) => new()
    {
        Success = false,
        ErrorCode = code,
        Error = message
    };
}
