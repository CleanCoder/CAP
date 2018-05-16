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
        public void ReceiveMessage(FlowContext<DateTime> time)
        {
            System.Diagnostics.Debug.WriteLine("----- [A] message received: " + DateTime.Now + ",sent time: " + time.Content);

            _capBus.Publish("B", time.ToNextStep(DateTime.Now));
        }

        [CapSubscribe("B.Rollback")]
        public void Rollback(FlowContext<DateTime> time)
        {
            System.Diagnostics.Debug.WriteLine("---- [B] rollback message received: " + DateTime.Now + ",sent time: " + time.Content);

            _capBus.Publish(string.Empty, time.RollbackStep(DateTime.Now));
        }
    }
}
