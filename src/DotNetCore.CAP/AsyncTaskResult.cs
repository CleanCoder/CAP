using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.CAP
{
    public enum AsyncTaskStatus
    {
        Success,
        InProgress,
        Failed
    }

    /// <summary>Represents an async task result.
    /// </summary>
    public class AsyncTaskResult
    {
        public readonly static AsyncTaskResult Success = new AsyncTaskResult(AsyncTaskStatus.Success, string.Empty);
        public readonly static AsyncTaskResult InProgess = new AsyncTaskResult(AsyncTaskStatus.InProgress, string.Empty);

        public AsyncTaskResult() { }

        public AsyncTaskResult(AsyncTaskStatus status, string errorMessage)
        {
            Status = status;
            ErrorMessage = errorMessage;
        }

        public AsyncTaskStatus Status { get; set; }

        public string ErrorMessage { get; set; }
    }

    /// <summary>Represents a generic async task result.
    /// </summary>
    public class AsyncTaskResult<TResult> : AsyncTaskResult
    {
        // TODO: ugly solution
        public readonly static new AsyncTaskResult<TResult> InProgess = new AsyncTaskResult<TResult>(AsyncTaskStatus.InProgress, string.Empty, default(TResult));

        public TResult Data { get; set; }

        public AsyncTaskResult() { }

        public AsyncTaskResult(bool isSucceeded) : this(isSucceeded, string.Empty, default(TResult))
        {
        }

        public AsyncTaskResult(bool isSucceeded, TResult data) : this(isSucceeded, string.Empty, data)
        {
        }

        public AsyncTaskResult(bool isSucceeded, string errorMessage) : this(isSucceeded, errorMessage, default(TResult))
        {
        }

        public AsyncTaskResult(bool isSucceeded, string errorMessage, TResult data): base(isSucceeded ? AsyncTaskStatus.Success : AsyncTaskStatus.Failed, errorMessage)
        {
            Data = data;
        }

        public AsyncTaskResult(AsyncTaskStatus status, string errorMessage, TResult data) : base(status, errorMessage)
        {
            Data = data;
        }

        public AsyncTaskResult(AsyncTaskResult source) : base(source.Status, source.ErrorMessage)
        {
        }
    }
}
