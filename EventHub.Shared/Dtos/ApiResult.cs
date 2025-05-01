using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Shared.Dtos
{
    public record ApiResult(bool IsSuccess, string? Error)
    {
        public static ApiResult Success() => new(true, null);
        public static ApiResult Fail(string error) => new(false, error);
    }

    public record ApiResult<T>(bool IsSuccess, T Data, string? Error)
    {
        public static ApiResult<T> Success(T data) => new(true, data, null);
        public static ApiResult<T> Fail(string error) => new(false, default!, error);
    }
}
