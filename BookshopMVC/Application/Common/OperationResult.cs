using System.Diagnostics.CodeAnalysis;

namespace BookshopMVC.Application.Common
{
    public class OperationResult
    {
        public bool Success { get; init; }
        public string? ErrorCode { get; init; }
        public string? Message { get; init; }

        public static OperationResult Ok(string? message = null) => new OperationResult { Success = true, Message = message };
        public static OperationResult Fail(string errorCode, string? message = null) => new OperationResult { Success = false, ErrorCode = errorCode, Message = message };
    }

    public class OperationResult<T>
    {
        public bool Success { get; init; }
        public string? ErrorCode { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }

        [MemberNotNullWhen(true, nameof(Data))]
        public bool HasData => Success && Data is not null;

        public static OperationResult<T> Ok(T data, string? message = null) => new OperationResult<T> { Success = true, Data = data, Message = message };
        public static OperationResult<T> Fail(string errorCode, string? message = null) => new OperationResult<T> { Success = false, ErrorCode = errorCode, Message = message };
    }
}
