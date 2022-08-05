using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using HousingRepairsOnline.AddressIngestion.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace HousingRepairsOnline.AddressIngestion
{
    public static class Function
    {
        [FunctionName("Function")]
        public static async Task RunAsync(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Blob("insight-report-addresses/communal.csv", FileAccess.Read,  Connection = "AzureWebJobsStorage")] Stream inputStream,
            // [CosmosDB(
            //     databaseName: "TestAddresses",
            //     collectionName: "Communal",
            //     ConnectionStringSetting = "CosmosDBConnection")]
            // IAsyncCollector<CommunalAddresses> communalAddressesOut,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
            log.LogInformation(inputStream.Length.ToString());

            using var reader = new StreamReader(inputStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<CommunalAddresses>();
            log.LogInformation(records.First().ToString());
        }
    }
}