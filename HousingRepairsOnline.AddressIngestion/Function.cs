using System;
using System.IO;
using System.Threading.Tasks;
using HousingRepairsOnline.AddressIngestion.Helpers;
using HousingRepairsOnline.AddressIngestion.Services;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using HACT.Dtos;

namespace HousingRepairsOnline.AddressIngestion
{
    public static class Function
    {
        [FunctionName("IngestAddresses")]
        public static async Task RunAsync(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Blob("%BlobPath%", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream inputStream,
            [CosmosDB(
                databaseName : "%DatabaseName%",
                collectionName: "%CollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<PropertyAddress> propertyAddressesOut,
            [CosmosDB(
                databaseName : "%DatabaseName%",
                collectionName: "%CollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            var blobPath = EnvironmentVariableHelper.GetEnvironmentVariable("BlobPath");

            if (inputStream == null)
            {
                throw new FileLoadException($"File '{blobPath}' does not exist in the container");
            }

            var databaseName = EnvironmentVariableHelper.GetEnvironmentVariable("DatabaseName");
            var collectionName = EnvironmentVariableHelper.GetEnvironmentVariable("CollectionName");
            var partitionKey = EnvironmentVariableHelper.GetEnvironmentVariable("PartitionKey");

            var recreateDocumentCollection = new RecreateDocumentCollection(client);
            var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
            var databaseUri = UriFactory.CreateDatabaseUri(databaseName);
            await recreateDocumentCollection.Execute(databaseUri, collectionUri, collectionName, partitionKey);

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            var addresses = Mapper.CsvInputStreamToAddresses(inputStream);
            var propertyAddresses = Mapper.ToHactPropertyAddresses(addresses);
            var insertAddressesToCosmosDB = new InsertAddressesToCosmosDb(propertyAddressesOut);
            await insertAddressesToCosmosDB.Execute(propertyAddresses);
        }
    }
}
