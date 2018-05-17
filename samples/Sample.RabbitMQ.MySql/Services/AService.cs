using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.RabbitMQ.MySql.Services
{
    public class AService : ICapSubscribe
    {
        private readonly ICapPublisher _capBus;

        public AService(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [CapSubscribe("A")]
        public void ReceiveMessage(FlowContext<string> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [A] message received: " + DateTime.Now);

            var nextMessage = string.Join(" -> ", flowContext.Messge, "A");
            _capBus.Publish("B", flowContext.Forward(nextMessage));
        }

        [CapSubscribe("B.Completed")]
        public void Rollback(FlowContext<string, IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("---- [B.Completed] message received: " + DateTime.Now + ",sent time: " + flowContext.Messge);

            if (flowContext.Result.Succeeded)
            {
                _capBus.Publish(string.Empty, flowContext.MarkComplete<string, IEnumerable<string>>(flowContext.Result.Data.Append("B.Complete")));
            }
            else
            {
                _capBus.Publish(string.Empty, flowContext.RollBack<string, IEnumerable<string>>(flowContext.Messge + " -> A.Rollback", string.Empty));
            }
        }
    }
}