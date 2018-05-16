using System;
using System.Collections.Generic;
using System.Linq;
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
            _capBus.Publish("A", FlowContext<List<string>>.Start(new List<string>() { "A" }));

            return Ok(DateTime.Now);
        }

        [CapSubscribe("A.Rollback")]
        public void Rollback(FlowContext<IEnumerable<string>> flowContext)
        {
            var workflow = flowContext.Content.Append("A.Rollback");
            System.Diagnostics.Debug.WriteLine("###########################   Finish: " + string.Join(" -> ", workflow));
        }

        [Route("~/publish2")]
        public IActionResult PublishMessage2()
        {
            _capBus.Publish("D", FlowContext<List<string>>.Start(new List<string>() { "D" }));

            return Ok(DateTime.Now);
        }

        [CapSubscribe("D.Rollback")]
        public void RollbackD(FlowContext<IEnumerable<string>> flowContext)
        {
            var workflow = flowContext.Content.Append("D.Rollback");
            System.Diagnostics.Debug.WriteLine("###################### Finish: " + string.Join(" -> ", workflow));
        }
    }
}
