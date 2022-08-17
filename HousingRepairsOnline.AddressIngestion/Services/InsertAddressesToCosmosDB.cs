using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using HousingRepairsOnline.AddressIngestion.Domain;
using Microsoft.Azure.WebJobs;

namespace HousingRepairsOnline.AddressIngestion.Services;

public class InsertAddressesToCosmosDB
{
    private readonly IAsyncCollector<CommunalAddress> communalAddressesOut;

    public InsertAddressesToCosmosDB(IAsyncCollector<CommunalAddress> communalAddressesOut)
    {
        this.communalAddressesOut = communalAddressesOut;
    }

    public async Task Execute(IEnumerable<CommunalAddress> addresses)
    {
        foreach (var address in addresses)
        {
            Guard.Against.NullOrEmpty(address.AddressLine);
            Guard.Against.NullOrEmpty(address.PostCode);
            Guard.Against.Null(address.PlaceReference);

            await communalAddressesOut.AddAsync(address);
        }
    }
}