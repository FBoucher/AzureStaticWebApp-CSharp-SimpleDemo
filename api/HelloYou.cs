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

namespace Company.Function
{
    public static class HelloYou
    {
        [FunctionName("HelloYou")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "secured/HelloYou")] HttpRequest req,
            ILogger log,
            ClaimsPrincipal principal)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            bool isClaimValid = true;

            if (principal == null)
            {
                log.LogWarning("No principal.");
                isClaimValid = false;
            }

            if (isClaimValid && principal.Identity == null)
            {
                log.LogWarning("No identity.");
                isClaimValid = false;
            }

            if (isClaimValid && !principal.Identity.IsAuthenticated)
            {
                log.LogWarning("Request was not authenticated.");
                isClaimValid = false;
            }

            if (isClaimValid && principal.FindFirst(ClaimTypes.GivenName) is null)
            {
                log.LogError("Claim not Found");
                isClaimValid = false;
            }

            if(isClaimValid)
            {
                var userId = principal.FindFirst(ClaimTypes.GivenName).Value;
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
