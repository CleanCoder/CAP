using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace Sample.RabbitMQ.MySql.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ICapPublisher _capBus;

        public ValuesController(AppDbContext dbContext, ICapPublisher capPublisher)
        {
            _dbContext = dbContext;
            _capBus = capPublisher;
        }

        [Route("~/publish")]
        public IActionResult PublishMessage()
        {
            _capBus.Publish("A", FlowContext<DateTime>.Start(DateTime.Now));

            return Ok(DateTime.Now);
        }

        [Route("~/publish2")]
        public IActionResult PublishMessage2()
        {
            _capBus.Publish("D", FlowContext<DateTime>.Start(DateTime.Now));

            return Ok(DateTime.Now);
        }

        [CapSubscribe("A.Rollback")]
        public void Rollback(FlowContext<DateTime> time)
        {
           System.Diagnostics.Debug.WriteLine("---- [A] rollback message received: " + DateTime.Now + ",sent time: " + time.Content);
        }

        [CapSubscribe("D.Rollback")]
        public void RollbackD(FlowContext<DateTime> time)
        {
            System.Diagnostics.Debug.WriteLine("---- [D] rollback message received: " + DateTime.Now + ",sent time: " + time.Content);
        }
    }
}
