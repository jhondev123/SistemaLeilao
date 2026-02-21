using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public SucessMessage? Message { get; }
        public IEnumerable<ErrorMessage>? Errors { get; }
        public IEnumerable<string>? ErrorsWithoutCode { get; }

        protected Result(bool isSuccess, SucessMessage? message, IEnumerable<ErrorMessage>? errors)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors;
        }

        public static Result Success(SucessMessage message) => new(true, message, null);
        public static Result Failure(IEnumerable<ErrorMessage> errors) => new(false, null, errors);
        public static Result Failure(IEnumerable<string> errors)
        {
            var errorList = new List<ErrorMessage>();
            foreach (var error in errors)
            {
                errorList.Add(new ErrorMessage(string.Empty, error));
            }
            return new Result(false, null, errorList);
        }
        public static Result Failure(ErrorMessage error) => new(false, null, new List<ErrorMessage> { error });
    }
    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, T? data, SucessMessage? message, IEnumerable<ErrorMessage>? errors)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data, SucessMessage message) => new(true, data, message, null);
        public static new Result<T> Failure(IEnumerable<ErrorMessage> errors) => new(false, default, null, errors);
        public static new Result<T> Failure(ErrorMessage error) => new(false, default, null, new List<ErrorMessage> { error });
    }
}
