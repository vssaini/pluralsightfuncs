using System;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace pluralsightfuncs
{
    public static class EmailLicenseFile
    {
        [FunctionName("EmailLicenseFile")]
        public static void Run([BlobTrigger("licenses/{name}", Connection = "AzureWebJobsStorage")] string licenseFileContents,
            [SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message,
            string name, ILogger log)
        {
            var email = Regex.Match(licenseFileContents, @"^Email\:\ (.+)$", RegexOptions.Multiline).Groups[1].Value;
            log.LogInformation($"Got order from {email}\n License file Name:{name}");

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);

            message = new SendGridMessage
            {
                From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender")),
                Subject = "Your license file",
                HtmlContent = "Thank you for your order"
            };
            message.AddTo(email);
            message.AddAttachment(name, base64, "text/plain");

            //if (!email.EndsWith("@test.com"))
            //    sender.Add(message);
        }
    }
}
