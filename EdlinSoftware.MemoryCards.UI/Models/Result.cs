using System;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    internal abstract class Result
    {
        private readonly string _errorMessage;

        public ResultStatus Status { get; }

        protected Result(ResultStatus status, string errorMessage = null)
        {
            Status = status;
            _errorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get
            {
                if (Status == ResultStatus.Success)
                { throw new InvalidOperationException(); }

                return _errorMessage;
            }
        }
    }

    internal abstract class Result<T> : Result
    {
        private readonly T _value;

        protected Result(ResultStatus status, T value, string errorMessage = null)
            : base(status, errorMessage)
        {
            _value = value;
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

    internal class Success : Result
    {
        public Success() : base(ResultStatus.Success)
        {}
    }

    internal class Failure : Result
    {
        public Failure(string errorMessage) : base(ResultStatus.Failure, errorMessage)
        { }
    }

    internal class Success<T> : Result<T>
    {
        public Success(T value) : base(ResultStatus.Success, value)
        { }
    }

    internal class Failure<T> : Result<T>
    {
        public Failure(string errorMessage) : base(ResultStatus.Failure, default(T), errorMessage)
        { }
    }
}