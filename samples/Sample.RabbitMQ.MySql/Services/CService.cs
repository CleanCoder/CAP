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
        public void ReceiveMessage(FlowContext<string> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [C] message received: " + DateTime.Now);

            var nextMessage = string.Join(" -> ", flowContext.Messge, "C");
            _capBus.Publish("B1", flowContext.Forward(nextMessage));
        }

        [CapSubscribe("B1.Completed")]
        public void ReceiveMessageB(FlowContext<string, IEnumerable<string>> flowContext)
        {
            if (flowContext.IsSucessed)
            {
                _capBus.Publish(string.Empty, flowContext.MarkComplete<string, IEnumerable<string>>(flowContext.Result.Data.Append("B1.Complete")));
            }
            else
            {
                _capBus.Publish(string.Empty, flowContext.RollBack<string, IEnumerable<string>>(flowContext.Messge + " -> B1.Rollback"));
            }
        }
    }
}