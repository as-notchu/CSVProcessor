using CSVProcessor.Enum;

namespace CSVProcessor.Models;

public class WarningsDetails
{
    public Operations Operation { get; set; }
    
    public string Message { get; set; }

    public WarningsDetails(Operations operation, string message)
    {
        Operation = operation;
        Message = message;
    }
}