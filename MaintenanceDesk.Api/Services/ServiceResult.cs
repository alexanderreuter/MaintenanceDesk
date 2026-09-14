using System.Diagnostics.CodeAnalysis;

namespace MaintenanceDesk.Api.Services;

public sealed class ServiceResult<T> where T : class
{
    private ServiceResult(ResultStatus status, T? value, string? error)
    {
        Status = status;
        Value = value;
        Error = error;
    }

    public ResultStatus Status { get; }

    public T? Value { get; }

    public string? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Status == ResultStatus.Success;

    public static ServiceResult<T> Success(T value) => new(ResultStatus.Success, value, null);

    public static ServiceResult<T> NotFound(string error) => new(ResultStatus.NotFound, null, error);

    public static ServiceResult<T> Invalid(string error) => new(ResultStatus.Invalid, null, error);

    public static ServiceResult<T> Conflict(string error) => new(ResultStatus.Conflict, null, error);
}
