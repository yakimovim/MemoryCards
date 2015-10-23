using System;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    internal class Result
    {
        protected string _errorMessage;

        public ResultStatus Status { get; protected set; }

        public string ErrorMessage
        {
            get
            {
                if (Status == ResultStatus.Success)
                { throw new InvalidOperationException(); }

                return _errorMessage;
            }
        }


        public static Result CreateSuccess()
        {
            var result = new Result
            {
                Status = ResultStatus.Success,
            };
            return result;
        }

        public static Result CreateFailure(string errorMessage)
        {
            var result = new Result
            {
                Status = ResultStatus.Failure,
                _errorMessage = errorMessage
            };
            return result;
        }
    }

    internal class Result<T> : Result
    {
        protected T _value;

        private Result()
        {}

        public static Result<T> CreateSuccess(T value)
        {
            var result = new Result<T>
            {
                Status = ResultStatus.Success,
                _value = value
            };
            return result;
        }

        public new static Result<T> CreateFailure(string errorMessage)
        {
            var result = new Result<T>
            {
                Status = ResultStatus.Failure,
                _errorMessage = errorMessage
            };
            return result;
        }

        public T Value
        {
            get
            {
                if (Status == ResultStatus.Failure)
                { throw new InvalidOperationException(); }

                return _value;
            }
        }
    }

    internal enum ResultStatus
    {
        Success,
        Failure
    }
}