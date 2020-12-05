using System;
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
        [Route("metrics")]
        [HttpGet]
        public IEnumerable<string> Get(string task)
        {
            Random rnd = new Random();
            var array = new List<Tuple<string,int,int>>();
            array.Add(new Tuple<string, int, int>("01:00",  rnd.Next(1,1000), rnd.Next(1,400)));
            array.Add(new Tuple<string, int, int>("01:05",  rnd.Next(1,1000), rnd.Next(1,400)));
            array.Add(new Tuple<string, int, int>("01:10",  rnd.Next(1,1000), rnd.Next(1,400)));
            array.Add(new Tuple<string, int, int>("01:20",  rnd.Next(1,1000), rnd.Next(1,400)));

            foreach(var a in array)
            {
                yield return $"{a.Item1},{a.Item2},{a.Item3}";
            }
        }
    }
}
