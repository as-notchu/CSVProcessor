using CSVProcessor.Enum;

namespace CSVProcessor.Models;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ServiceErrorCodes ErrorCode { get; set; } = ServiceErrorCodes.None;
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ServiceResult<T> Fail(ServiceErrorCodes codes, string message) => new()
    {
        Success = false,
        ErrorCode = codes,
        Error = message
    };
}
