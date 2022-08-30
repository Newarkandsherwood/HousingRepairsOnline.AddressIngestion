namespace HousingRepairsOnline.AddressIngestion.Services
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Microsoft.Azure.Documents;

    public class RecreateDocumentCollectionInCosmos
    {
        private readonly IDocumentClient documentClient;

        public RecreateDocumentCollectionInCosmos(IDocumentClient documentClient) => this.documentClient = documentClient;

        public async Task Execute(Uri databaseUri, Uri collectionUri, string collectionName, string partitionKey)
        {
            Guard.Against.NullOrEmpty(collectionName, nameof(collectionName));
            Guard.Against.NullOrEmpty(partitionKey, nameof(partitionKey));


            await this.documentClient.DeleteDocumentCollectionAsync(collectionUri);
            await this.documentClient.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection
            {
                Id = collectionName,
                PartitionKey = new PartitionKeyDefinition
                {
                    Paths = new Collection<string> { partitionKey }
                }
            });
        }
    }
}
