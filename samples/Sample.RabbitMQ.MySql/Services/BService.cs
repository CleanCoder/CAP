using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.RabbitMQ.MySql.Services
{
    public class BService : ICapSubscribe
    {
        private readonly ICapPublisher _capBus;

        public BService(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [CapSubscribe("B")]
        public void ReceiveMessage(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [B] message received: " + DateTime.Now);

            _capBus.Publish("C",  flowContext.ToNextStep(flowContext.Content.Append("C")));
        }

        [CapSubscribe("C.Rollback")]
        public void Rollback(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("---- [C] rollback message received: " + DateTime.Now);

            _capBus.Publish(string.Empty, flowContext.RollbackStep(flowContext.Content.Append("C.Rollback")));
        }


        [CapSubscribe("B1")]
        public void ReceiveMessageC(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [B1] message received: " + DateTime.Now);

            _capBus.Publish(string.Empty, flowContext.RollbackStep(flowContext.Content.Append("Rollback")));
        }
    }
}
