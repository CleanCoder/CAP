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
        public void ReceiveMessage(FlowContext<DateTime> time)
        {
            System.Diagnostics.Debug.WriteLine("----- [C] message received: " + DateTime.Now + ",sent time: " + time.Content);

            System.Diagnostics.Debug.WriteLine("----- Something bad happened. Rollback...");

            _capBus.Publish(string.Empty, time.RollbackStep(DateTime.Now));
        }
    }
}
