using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.CAP
{
    /// <summary>Represents an async task result.
    /// </summary>
    public class AsyncTaskResult
    {
        public readonly static AsyncTaskResult Success = new AsyncTaskResult(true, string.Empty);

        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; }

        public AsyncTaskResult() { }

        public AsyncTaskResult(bool isSucceeded, string errorMessage)
        {
            Succeeded = isSucceeded;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>Represents a generic async task result.
    /// </summary>
    public class AsyncTaskResult<TResult> : AsyncTaskResult
    {
        public TResult Data { get; set; }

        public AsyncTaskResult() { }

        public AsyncTaskResult(bool isSucceeded) : this(isSucceeded, null, default(TResult))
        {
        }

        public AsyncTaskResult(bool isSucceeded, TResult data) : this(isSucceeded, null, data)
        {
        }

        public AsyncTaskResult(bool isSucceeded, string errorMessage) : this(isSucceeded, errorMessage, default(TResult))
        {
        }

        public AsyncTaskResult(bool isSucceeded, string errorMessage, TResult data): base(isSucceeded, errorMessage)
        {
            Data = data;
        }
    }
}
