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
            _capBus.Publish("A", FlowContext<string>.Start("Client"));

            return Ok(DateTime.Now);
        }

        [CapSubscribe("A.Completed")]
        public void ACompleted(FlowContext<string, IEnumerable<string>> flowContext)
        {
            System.Diagnostics.Debug.WriteLine("###########################   TraceInfo: " + string.Join(" -> ", flowContext.Infos));

            if (flowContext.IsSucessed)
            {
                System.Diagnostics.Debug.WriteLine("###########################   Finish: " + string.Join(" -> ", flowContext.Result.Data.Append("A.Complete")));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("###########################   Rollback: " + flowContext.Messge);
            }
        }

        [Route("~/publish2")]
        public IActionResult PublishMessage2()
        {
            _capBus.Publish("D", FlowContext<List<string>>.Start(new List<string>() { "D" }));

            return Ok(DateTime.Now);
        }

        [CapSubscribe("D.Completed")]
        public void RollbackD(FlowContext<string, IEnumerable<string>> flowContext)
        {
            var workflow = flowContext.Result.Data.Append("D.Rollback");
            System.Diagnostics.Debug.WriteLine("###################### Finish: " + string.Join(" -> ", workflow));
        }
    }
}
