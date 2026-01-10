namespace DreamShelf.API.Common.Dtos.Response;

public class Result<T>
{
    public bool isSuccess { get; set; }
    public T Value { get; set; }
    public string? Message  { get; set; }
    public ErrorType? ErrorType { get; set; }
}

public class Result 
{
    public bool isSuccess { get; set; }
    public string? Message { get; set; }
    public ErrorType? ErrorType { get; set; }
}

public enum ErrorType
{
    Unauthorized,
    NotFound,
    ServerError
}