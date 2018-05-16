using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.RabbitMQ.MySql.Services
{
    public class DService : ICapSubscribe
    {
        private readonly ICapPublisher _capBus;

        public DService(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [CapSubscribe("D")]
        public void ReceiveMessage(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [A] message received: " + DateTime.Now);

            _capBus.Publish("B", flowContext.ToNextStep(flowContext.Content.Append("B")));
        }

        [CapSubscribe("B.Rollback")]
        public void Rollback(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("---- [B] rollback message received: " + DateTime.Now);

            _capBus.Publish(string.Empty, flowContext.RollbackStep(flowContext.Content.Append("B.Rollback")));
        }
    }
}
