using CSVProcessor.Enum;

namespace CSVProcessor.Models;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public DataServiceErrorCode ErrorCode { get; set; } = DataServiceErrorCode.None;
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ServiceResult<T> Fail(DataServiceErrorCode code, string message) => new()
    {
        Success = false,
        ErrorCode = code,
        Error = message
    };
}
