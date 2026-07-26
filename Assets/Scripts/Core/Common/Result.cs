namespace SReader.Core.Common
{
    /// <summary>
    /// Outcome of an operation that can fail — services and repositories
    /// return Result instead of throwing, so ViewModels can surface the
    /// error text directly in the UI's error-label slots.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error ?? string.Empty;
        }

        public static Result Ok() => new Result(true, null);
        public static Result Fail(string error) => new Result(false, error);

        public static Result<T> Ok<T>(T value) => new Result<T>(true, null, value);
        public static Result<T> Fail<T>(string error) => new Result<T>(false, error, default);
    }

    /// <summary>A Result that also carries a value on success.</summary>
    public class Result<T> : Result
    {
        public T Value { get; }

        internal Result(bool isSuccess, string error, T value) : base(isSuccess, error)
        {
            Value = value;
        }
    }
}
