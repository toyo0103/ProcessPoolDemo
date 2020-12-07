using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dashboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        //time, process count, queue length
        static ConcurrentDictionary<string,List<Tuple<string, int, int>>> MetricDataStorage = 
            new ConcurrentDictionary<string, List<Tuple<string, int, int>>>();


        [Route("metrics")]
        [HttpGet]
        public IEnumerable<string> Get(string task)
        {
           if(MetricDataStorage.TryGetValue(task,out var datas))
           {
               foreach(var data in datas)
               {
                    yield return $"{data.Item1},{data.Item2},{data.Item3}";
               }
           }
           yield break;
        }

        [Route("metrics")]
        [HttpPost]
        public ActionResult SetMetricValue(SetMetricValueParameter parameter)
        {
            if(MetricDataStorage.ContainsKey(parameter.TaskName) == false)
            {
                MetricDataStorage.TryAdd(parameter.TaskName, new List<Tuple<string, int, int>>());
            }

            MetricDataStorage[parameter.TaskName].Add(new Tuple<string, int, int>(
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                parameter.ProcessCount,
                parameter.QueueLength
            ));
            return Ok();
        }
    }
}
