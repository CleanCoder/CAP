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
        public string CorrelationId { get; set; }

        public int Step { get; set; } = 1;

        public FlowDirection Direction { get; set; } = FlowDirection.Forward;
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
        public AsyncTaskResult<TResult> Result { get; set; }
    }

    public static class FlowContextExtend
    {
        public static FlowContext<TMessage> Forward<TMessage>(this FlowContext currentStep, TMessage message)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            return CopyFrom<TMessage, object>(currentStep, message, true);
        }

        public static FlowContext<TMessage, TResult> MarkComplete<TMessage, TResult>(this FlowContext currentStep, TResult result)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            // TODO: add status check 
            //if (currentStep.Direction != FlowDirection.Forward)
            //    throw new ArgumentException($"Only forward flow support MarkComplete. CorrelationId: {currentStep.CorrelationId}; Step: {currentStep.Step.ToString()}");

            FlowContext<TMessage, TResult> flowContext = CopyFrom<TMessage, TResult>(currentStep, default(TMessage), false);
            flowContext.Result = new AsyncTaskResult<TResult>(true, string.Empty, result);

            return flowContext;
        }

        public static FlowContext<TMessage, TResult> RollBack<TMessage, TResult>(this FlowContext currentStep, TMessage message, string reason)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            // TODO: add status check 
            //if (currentStep.Direction != FlowDirection.Forward)
            //    throw new ArgumentException($"Only forward flow support Rollback. CorrelationId: {currentStep.CorrelationId}; Step: {currentStep.Step.ToString()}");

            FlowContext<TMessage, TResult> flowContext = CopyFrom<TMessage, TResult>(currentStep, message, false);
            flowContext.Result = new AsyncTaskResult<TResult>(false, reason);

            return flowContext;
        }

        private static FlowContext<TMessage, TResult> CopyFrom<TMessage, TResult>(FlowContext source, TMessage message, bool isPositive)
        {
            FlowContext<TMessage, TResult> instance = new FlowContext<TMessage, TResult>();
            instance.CorrelationId = source.CorrelationId;
            instance.Step = source.Step + (isPositive ? 1 : (source.Direction == FlowDirection.Backward ? -1 : 0));
            instance.Messge = message;
            instance.Direction = isPositive ? FlowDirection.Forward : FlowDirection.Backward;

            return instance;
        }
    }
}
