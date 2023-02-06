using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("/")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("api invoked");
            MyDictionary vs = new MyDictionary();
            try
            {
                foreach (KeyValuePair<string, StringValues> h in HttpContext.Request.Headers)
                {
                    vs.Add(h.Key, h.Value.ToString());
                }
                foreach (KeyValuePair<string, string> h in HttpContext.Request.Cookies)
                {
                    vs.Add(h.Key, h.Value);
                }
                foreach (KeyValuePair<string, StringValues> h in HttpContext.Response.Headers)
                {
                    vs.Add(h.Key, h.Value.ToString());
                }
                foreach (var h in HttpContext.Items)
                {
                    vs.Add(h.Key.ToString(), h.Value.ToString());
                }
                foreach (var h in HttpContext.Session.Keys)
                {
                    vs.Add(new Random().Next().ToString(), h);
                }
            }
            catch(Exception ex) 
            {
                vs.Add("exceprion", ex.Message);
            }

            vs.LoadServerVariables(HttpContext);
            vs.LoadRawHttp(HttpContext);
            vs.DnsData(HttpContext);
            vs.LoadResponseHeader(Response);
            vs.Add("HOST_VALUE", HttpContext.Request.Host.Value);
            vs.LoadRequestHeader(Request);
            String hostName = string.Empty;
            hostName = Dns.GetHostName();
            IPHostEntry myIP = Dns.GetHostEntry(hostName);
            IPAddress[] address = myIP.AddressList;
            for (int i = 0; i < address.Length; i++)
            {
                vs.Add($"IP Address {i} : ", address[i].ToString());
                try
                {
                    vs.Add($"IP HostName {i}", Dns.GetHostEntry(address[i]).HostName);
                }
                catch { }
                try
                {
                    vs.Add($"IP aliases {i}", string.Join(',', Dns.GetHostEntry(address[i]).Aliases));
                }
                catch { }
            }
            vs.Get_MAC_Address();
            
            return Content(vs.ToString(), "text/html", Encoding.UTF8);
        }
    }
}
