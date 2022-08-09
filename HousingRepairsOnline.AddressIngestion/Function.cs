using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using HousingRepairsOnline.AddressIngestion.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HousingRepairsOnline.AddressIngestion
{
    public static class Function
    {
        [FunctionName("IngestAddresses")]
        public static async Task RunAsync(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Blob("%CommunalBlobPath%", FileAccess.Read,  Connection = "AzureWebJobsStorage")] Stream inputStream,
            [CosmosDB(
                databaseName : "%DatabaseName%",
                collectionName: "%CollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<CommunalAddresses> communalAddressesOut,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            using var reader = new StreamReader(inputStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<CommunalAddresses>();
            foreach (var record in records)
            {
                await communalAddressesOut.AddAsync(record);
            }
        }
    }
}