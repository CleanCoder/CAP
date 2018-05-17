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
        private Random _random = new Random((int)DateTime.UtcNow.Ticks);

        public BService(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        [CapSubscribe("B")]
        public void ReceiveMessage(FlowContext<string> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [B] message received: " + DateTime.Now);

            var nextMessage = string.Join(" -> ", flowContext.Messge, "B");
            _capBus.Publish("C", flowContext.Forward(nextMessage));
        }

        [CapSubscribe("C.Completed")]
        public void Completed(FlowContext<string, IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("---- [C.Completed] message received: " + DateTime.Now);

            if (flowContext.IsSucessed)
            {
                _capBus.Publish(string.Empty, flowContext.MarkComplete<string, IEnumerable<string>>(flowContext.Result.Data.Append("C.Complete")));
            }
            else
            {
                _capBus.Publish(string.Empty, flowContext.RollBack<string, IEnumerable<string>>(flowContext.Messge + " -> C.Rollback"));
            }
        }

        [CapSubscribe("B1")]
        public void ReceiveMessageC(FlowContext<string> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("----- [B1] message received: " + DateTime.Now);
            var nextMessage = string.Join(" -> ", flowContext.Messge, "B1");

            var value = _random.Next(0, 2);
            if (value < 1)
            {
                flowContext.AppendInfo("Completed in B1");
                _capBus.Publish(string.Empty, flowContext.MarkComplete<string, IEnumerable<string>>(nextMessage.Split(" -> ")));
            }
            else
            {
                flowContext.AppendInfo("Rollback in B1");
                _capBus.Publish(string.Empty, flowContext.RollBack<string, IEnumerable<string>>(nextMessage + " -> B1.Rollback", "Just for test"));
            }
        }
    }
}