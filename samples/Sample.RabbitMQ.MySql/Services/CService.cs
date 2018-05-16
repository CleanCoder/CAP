using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.RabbitMQ.MySql.Services
{
    public class CService : ICapSubscribe
    {
        private readonly ICapPublisher _capBus;

        public CService(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [CapSubscribe("C")]
        public void ReceiveMessage(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [C] message received: " + DateTime.Now);

            _capBus.Publish("B1", flowContext.ToNextStep(flowContext.Content.Append("B1")));
        }


        [CapSubscribe("B1.Rollback")]
        public void ReceiveMessageB(FlowContext<IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [B1.Rollback] message received: " + DateTime.Now);

            _capBus.Publish(string.Empty, flowContext.RollbackStep(flowContext.Content.Append("B1.Rollback")));
        }
    }
}
