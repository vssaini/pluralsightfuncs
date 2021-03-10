using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace pluralsightfuncs
{
    public static class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public static void Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")] Order order,
            [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob,
            ILogger log)
        {
            log.LogInformation($"Processing order with Id: {order.OrderId}");

            outputBlob.WriteLine($"OrderId: {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {DateTime.UtcNow}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(order.Email + "secret"));

            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");
        }
    }
}
