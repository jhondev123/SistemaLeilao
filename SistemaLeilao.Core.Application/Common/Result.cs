using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public IEnumerable<string>? Errors { get; }

        protected Result(bool isSuccess, string? message, IEnumerable<string>? errors)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors;
        }

        public static Result Success(string message = "") => new(true, message, null);
        public static Result Failure(IEnumerable<string> errors) => new(false, null, errors);
        public static Result Failure(string error) => new(false, null, new List<string> { error });
    }
    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, T? data, string? message, IEnumerable<string>? errors)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string message = "") => new(true, data, message, null);
        public static new Result<T> Failure(IEnumerable<string> errors) => new(false, default, null, errors);
        public static new Result<T> Failure(string error) => new(false, default, null, new List<string> { error });
    }
}
