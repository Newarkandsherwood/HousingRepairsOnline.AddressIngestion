using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace HousingRepairsOnline.AddressIngestion.Services;

public class RecreateDocumentCollection
{
    private readonly IDocumentClient documentClient;

    public RecreateDocumentCollection(IDocumentClient documentClient)
    {
        this.documentClient = documentClient;
    }

    public async Task Execute(Uri databaseUri, Uri collectionUri, string collectionName, string partitionKey)
    {
        Guard.Against.NullOrEmpty(collectionName, nameof(collectionName));
        Guard.Against.NullOrEmpty(partitionKey, nameof(partitionKey));


        await documentClient.DeleteDocumentCollectionAsync(collectionUri);
        await documentClient.CreateDocumentCollectionAsync(databaseUri, new DocumentCollection
        {
            Id = collectionName,
            PartitionKey = new PartitionKeyDefinition
            {
                Paths = new Collection<string> { partitionKey }
            }
        });
    }
}