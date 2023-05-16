namespace ProgrammerTest.Order.WebApi.Common;

public class ApiResponse<T>
{
    public ApiResponse(string message, bool success, T result)
    {
        Message = message;
        IsSuccess = success;
        Content = result;
    }

    public ApiResponse(string errorMsg)
    {
        Message = errorMsg;
        IsSuccess = false;
        Content = default;
    }

    public ApiResponse(T result)
    {
        Content = result;
        IsSuccess = true;
    }

    public string Message { get; set; }

    public bool IsSuccess { get; set; }

    public T Content { get; set; }

    public static implicit operator ApiResponse<T>(T value) => new(value);

    public static implicit operator ApiResponse<T>(string message) => new(message);
}