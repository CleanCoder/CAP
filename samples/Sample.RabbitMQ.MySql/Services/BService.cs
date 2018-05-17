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

            //  _capBus.Publish("C", flowContext.Forward(flowContext.Messge.Append("C")));

            var value = _random.Next(0, 2);
            if (value < 1)
                _capBus.Publish(string.Empty, flowContext.MarkComplete<string, IEnumerable<string>>(nextMessage.Split(" -> ")));
            else
                _capBus.Publish(string.Empty, flowContext.RollBack<string, IEnumerable<string>>(nextMessage  + " -> B.Rollback" , "Just for test"));
        }

        //[CapSubscribe("C.Completed")]
        //public void Completed(FlowContext<IEnumerable<string>> flowContext)
        //{
        //    System.Diagnostics.Debug.WriteLine("---- [C.Completed] message received: " + DateTime.Now);

        //    _capBus.Publish(string.Empty, flowContext.Backword(flowContext.Messge.Append(flowContext.Result.Succeeded ? "C.Complete" : "C.Rollback")));
        //}

        //[CapSubscribe("B1")]
        //public void ReceiveMessageC(FlowContext<IEnumerable<string>> flowContext)
        //{
        //    System.Diagnostics.Debug.WriteLine("----- [B1] message received: " + DateTime.Now);

        //     var value =_random.Next(0, 2);
        //    if (value < 1)
        //        _capBus.Publish(string.Empty, flowContext.MarkComplete(flowContext.Messge.Append("Completed")));
        //    else
        //        _capBus.Publish(string.Empty, flowContext.RollBack(flowContext.Messge.Append("Rollback"), "test rollback"));

        //}
    }
}