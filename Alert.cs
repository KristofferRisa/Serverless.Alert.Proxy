using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;

namespace Serverless.Alert.Proxy
{
    public static class Alert
    {
        [FunctionName("Alert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Alert proxy HTTP trigger function processed a request.");

            //reading the post data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            //rocket.chat webhook url
            var url = System.Environment.GetEnvironmentVariable("rocketchaturl");

            if(data.message == null)
            {
                return new BadRequestObjectResult("missing message in the request.");
            }

            log.LogDebug($"Message: {data.message}");
            var message = new RocketChatRequest();
            message.text = data.message;
            
            // adding message attachments
            if(data.imageUrl != null) 
            {
                message.attachments = new List<RocketChatAttachment>
                {
                    new RocketChatAttachment
                    {
                        title = data.title ?? "Title",
                        image_url = data.imageUrl,
                        title_link = data.ruleUrl,
                        text = data.message
                    }
                };
            }

            // http client that sends a new post request to rocket.chat
            using(var httpClient = new HttpClient())
            {
                var stringContent = new StringContent(
                    JsonConvert.SerializeObject(message)
                    , Encoding.UTF8
                    , "application/json");
                    
                var response = httpClient.PostAsync(
                    url
                    ,stringContent);
                
                return response.Result != null
                    ? (ActionResult)new OkObjectResult($"Result: {response.Result}")
                    : new BadRequestObjectResult("Missing response from Rocket.Chat.");
            }
        }

        public class RocketChatRequest 
        {
            public string text { get; set; }
            public List<RocketChatAttachment> attachments { get; set; }
        }

        public class RocketChatAttachment 
        {
            public string title { get; set; }
            public string title_link { get; set; }
            public string text { get; set; }
            public string image_url { get; set; }
            public string color { get; set; }
        }
    }
}
