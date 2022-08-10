using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using HousingRepairsOnline.AddressIngestion.Domain;
using HousingRepairsOnline.AddressIngestion.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
            [CosmosDB(
                databaseName : "%DatabaseName%",
                collectionName: "%CollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            var databaseName = EnvironmentVariableHelper.GetEnvironmentVariable("DatabaseName");
            var collectionName = EnvironmentVariableHelper.GetEnvironmentVariable("CollectionName");
            var partitionKey = EnvironmentVariableHelper.GetEnvironmentVariable("PartitionKey");
            var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
            var communalBlobPath = EnvironmentVariableHelper.GetEnvironmentVariable("CommunalBlobPath");
            
            if (inputStream == null)
            {
                throw new FileLoadException($"File '{communalBlobPath}' does not exist in the container");
            }
        
            var databaseUri = UriFactory.CreateDatabaseUri(databaseName);
            await client.DeleteDocumentCollectionAsync(collectionUri);
            await client.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection()
            {
                Id = collectionName,
                PartitionKey = new PartitionKeyDefinition
                {
                    Paths = new Collection<string> { partitionKey }
                } 
            });
        
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