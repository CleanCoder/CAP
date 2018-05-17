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

        //[CapSubscribe("D")]
        //public void ReceiveMessage(FlowContext<IEnumerable<string>> flowContext)
        //{
        //    System.Diagnostics.Debug.WriteLine("----- [A] message received: " + DateTime.Now);

        //    _capBus.Publish("B", flowContext.Forward(flowContext.Messge.Append("B")));
        //}

        //[CapSubscribe("B.Completed")]
        //public void Rollback(FlowContext<IEnumerable<string>> flowContext)
        //{
        //    System.Diagnostics.Debug.WriteLine("---- [B.Completed] message received: " + DateTime.Now);

        //    _capBus.Publish(string.Empty, flowContext.Backword(flowContext.Result.Succeeded ? "B.Complete" : "B.Rollback"));
        //}
    }
}
