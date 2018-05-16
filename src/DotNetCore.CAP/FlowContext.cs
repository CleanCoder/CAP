using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.CAP
{
    public enum FlowDirection : byte
    {
        Positive = 1,
        Negative = 2
    }

    public abstract class FlowContext
    {
        public string CorrelationId { get; set; }

        public int Step { get; set; } = 1;

        public FlowDirection Direction { get; set; } = FlowDirection.Positive;
    }

    public class FlowContext<T> : FlowContext
    {
        public T Content { get; set; }

        public static FlowContext<T> Start(T content)
        {
            FlowContext<T> instance = new FlowContext<T>();
            instance.CorrelationId = Guid.NewGuid().ToString();
            instance.Direction = FlowDirection.Positive;
            instance.Step = 1;
            instance.Content = content;

            return instance;
        }
    }

    public static class FlowContextExtend
    {
        public static FlowContext<T> ToNextStep<T>(this FlowContext currentStep, T payload)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            return CopyFrom(currentStep, payload, true);
        }

        public static FlowContext<T> RollbackStep<T>(this FlowContext currentStep, T payload)
        {
            if (currentStep == null)
                throw new ArgumentNullException(nameof(currentStep));

            return CopyFrom(currentStep, payload, false);
        }

        private static FlowContext<T> CopyFrom<T>(FlowContext source, T payload, bool isPositive)
        {
            FlowContext<T> instance = new FlowContext<T>();
            instance.CorrelationId = source.CorrelationId;
            instance.Step = source.Step + (isPositive ? 1 : (source.Direction == FlowDirection.Negative ? -1 : 0));
            instance.Content = payload;
            instance.Direction = isPositive ? FlowDirection.Positive : FlowDirection.Negative;

            return instance;
        }
    }

}
