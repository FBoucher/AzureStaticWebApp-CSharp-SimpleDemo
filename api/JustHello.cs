using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Security.Claims;

namespace demo.Function
{
    public static class JustHello
    {
        [FunctionName("JustHello")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
            ILogger log,
            ClaimsPrincipal principal)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            bool isClaimValid = true;
            string userId = string.Empty;

            if (principal == null)
            {
                log.LogWarning("No principal.");
                isClaimValid = false;
            }

            if(isClaimValid)
            {
                userId = principal.FindFirst(ClaimTypes.GivenName).Value;
                log.LogInformation("Authenticated user {user}.", userId);
            }

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This SECURED HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Bonjour Hi, {name}. This SECURED HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}