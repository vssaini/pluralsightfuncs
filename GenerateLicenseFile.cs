using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace pluralsightfuncs
{
    public static class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public static async Task Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")] Order order,
            IBinder binder,
            ILogger log)
        {
            log.LogInformation($"Processing order with Id: {order.OrderId}");

            var outputBlob = await binder.BindAsync<TextWriter>(new BlobAttribute($"licenses/{order.OrderId}.lic")
            {
                Connection = "AzureWebJobsStorage"
            });

            await outputBlob.WriteLineAsync($"OrderId: {order.OrderId}");
            await outputBlob.WriteLineAsync($"Email: {order.Email}");
            await outputBlob.WriteLineAsync($"ProductId: {order.ProductId}");
            await outputBlob.WriteLineAsync($"PurchaseDate: {DateTime.UtcNow}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(order.Email + "secret"));

            await outputBlob.WriteLineAsync($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");
        }
    }
}
