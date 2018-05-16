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
        public void ReceiveMessage(FlowContext<DateTime> time)
        {
            System.Diagnostics.Debug.WriteLine("----- [B] message received: " + DateTime.Now + ",sent time: " + time.Content);

            _capBus.Publish("C",  time.ToNextStep(DateTime.Now));
        }

        [CapSubscribe("C.Rollback")]
        public void Rollback(FlowContext<DateTime> time)
        {
            System.Diagnostics.Debug.WriteLine("---- [C] rollback message received: " + DateTime.Now + ",sent time: " + time.Content);

            _capBus.Publish(string.Empty, time.RollbackStep(DateTime.Now));
        }
    }
}
