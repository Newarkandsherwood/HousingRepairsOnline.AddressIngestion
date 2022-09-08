namespace HousingRepairsOnline.AddressIngestion
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HACT.Dtos;
    using HousingRepairsOnline.AddressIngestion.Helpers;
    using HousingRepairsOnline.AddressIngestion.Services;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public static class IngestAddresses
    {
        [FunctionName("IngestAddresses")]
        public static async Task RunAsync(
            [BlobTrigger("%BlobPath%", Connection = "AzureWebJobsStorage")] Stream inputStream,
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
            var housingProvider = EnvironmentVariableHelper.GetEnvironmentVariable("HousingProvider");

            var recreateDocumentCollection = new RecreateDocumentCollectionInCosmos(client);
            var collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
            var databaseUri = UriFactory.CreateDatabaseUri(databaseName);
            await recreateDocumentCollection.Execute(databaseUri, collectionUri, collectionName, partitionKey);

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            var addresses = Mapper.CsvInputStreamToAddresses(inputStream);
            var propertyAddresses = Mapper.ToHactPropertyAddresses(addresses, housingProvider);

            // var attributes = new Attribute[]
            // {
            //     new CosmosDBAttribute(databaseName, collectionName)
            // };
            // var propertyAddressesOut = await binder.BindAsync<IAsyncCollector<PropertyAddress>>(attributes.First());
            // var insertAddressesToCosmosDB = new InsertAddressesToCosmosDb(propertyAddressesOut, log);
            // await insertAddressesToCosmosDB.Execute(propertyAddresses);

            foreach (var propertyAddress in propertyAddresses)
            {
                var isValidData = true;
                if (propertyAddress.AddressLine.FirstOrDefault() is "" or null)
                {
                    log.LogInformation($"AddressLine for property {propertyAddress.Reference.ID} is null or empty");
                    isValidData = false;
                }
                if (string.IsNullOrEmpty(propertyAddress.PostalCode))
                {
                    log.LogInformation($"Postalcode for property {propertyAddress.PostalCode} is null or empty");
                    isValidData = false;
                }
                if (string.IsNullOrEmpty(propertyAddress.Reference.ID))
                {
                    log.LogInformation($"Property Id is null or empty");
                    isValidData = false;
                }
                if (isValidData)
                {
                    await propertyAddressesOut.AddAsync(propertyAddress);
                }
            }
        }
    }
}

