using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetCore.CAP
{
    public enum FlowDirection : byte
    {
        Forward = 1,
        Backward = 2
    }

    public abstract class FlowContext
    {
        public List<string> _traceInfo = new List<string>();

        public string CorrelationId { get; set; }

        public int Step { get; set; } = 1;

        public FlowDirection Direction { get; set; } = FlowDirection.Forward;

        public AsyncTaskResult Result { get; set; } = AsyncTaskResult.InProgess;

        public IReadOnlyList<string> Infos { get { return _traceInfo.AsReadOnly(); } }

        public void AppendInfo(string info)
        {
            _traceInfo.Add(string.Format("{0}{1}: {2}", Direction == FlowDirection.Forward ? "F" : "R", Step.ToString(), info));
        }
    }

    public class FlowContext<TMessage> : FlowContext
    {
        public TMessage Messge { get; set; }

        public static FlowContext<TMessage> Start(TMessage message)
        {
            FlowContext<TMessage> instance = new FlowContext<TMessage>();
            instance.CorrelationId = Guid.NewGuid().ToString();
            instance.Direction = FlowDirection.Forward;
            instance.Step = 1;
            instance.Messge = message;

            return instance;
        }
    }

    public class FlowContext<TMessage, TResult> : FlowContext<TMessage>
    {
        public new AsyncTaskResult<TResult> Result { get; set; }

        public bool IsSucessed => Result.Status == AsyncTaskStatus.Success;
    }

    public static class FlowContextExtension
    {
        public static FlowContext<TMessage> Forward<TMessage>(this FlowContext currentStep, TMessage message)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            if (currentStep.Result.Status != AsyncTaskStatus.InProgress)
                throw new ArgumentException($"Forward operation only support for a progressing workflow. CorrelationId: {currentStep.CorrelationId}; Step: {currentStep.Step.ToString()}");

            return CopyFrom<TMessage, object>(currentStep, message, true);
        }

        public static FlowContext<TMessage, TResult> MarkComplete<TMessage, TResult>(this FlowContext currentStep, TResult result)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            if (currentStep.Result.Status == AsyncTaskStatus.Failed)
                throw new ArgumentException($"Forbid to call MarkComplete for a failed flow. Please use Rollback operation. CorrelationId: {currentStep.CorrelationId}; Step: {currentStep.Step.ToString()}");

            FlowContext<TMessage, TResult> flowContext = CopyFrom<TMessage, TResult>(currentStep, default(TMessage), false);
            flowContext.Result = new AsyncTaskResult<TResult>(true, string.Empty, result);

            return flowContext;
        }

        public static FlowContext<TMessage, TResult> RollBack<TMessage, TResult>(this FlowContext currentStep, TMessage message, string errorMessage = "" )
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            if (currentStep.Result.Status == AsyncTaskStatus.Success)
                throw new ArgumentException($"The workflow has been successed, can't rollback. CorrelationId: {currentStep.CorrelationId}; Step: {currentStep.Step.ToString()}");

            FlowContext<TMessage, TResult> flowContext = CopyFrom<TMessage, TResult>(currentStep, message, false);
            flowContext.Result = new AsyncTaskResult<TResult>(false, currentStep.Result.ErrorMessage ?? errorMessage);

            return flowContext;
        }

        #region [ Impelment Details... ]
        private static FlowContext<TMessage, TResult> CopyFrom<TMessage, TResult>(FlowContext source, TMessage message, bool isPositive)
        {
            FlowContext<TMessage, TResult> instance = new FlowContext<TMessage, TResult>();
            instance.CorrelationId = source.CorrelationId;
            instance.Step = source.Step + (isPositive ? 1 : (source.Direction == FlowDirection.Backward ? -1 : 0));
            instance.Messge = message;
            instance.Direction = isPositive ? FlowDirection.Forward : FlowDirection.Backward;
            instance.Result = AsyncTaskResult<TResult>.InProgess;
            instance._traceInfo = new List<string>(source._traceInfo);

            return instance;
        }
        #endregion
    }
}
