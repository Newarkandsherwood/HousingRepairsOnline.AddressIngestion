using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.WebJobs;
using HACT.Dtos;

namespace HousingRepairsOnline.AddressIngestion.Services;

public class InsertAddressesToCosmosDb
{
    private readonly IAsyncCollector<PropertyAddress> propertyAddressesOut;

    public InsertAddressesToCosmosDb(IAsyncCollector<PropertyAddress> propertyAddressesOut)
    {
        this.propertyAddressesOut = propertyAddressesOut;
    }

    public async Task Execute(IEnumerable<PropertyAddress> propertyAddresses )
    {
        foreach (var propertyAddress in propertyAddresses)
        {
            Guard.Against.NullOrEmpty(propertyAddress.AddressLine);
            Guard.Against.NullOrEmpty(propertyAddress.PostalCode);
            Guard.Against.Null(propertyAddress.Reference);

            await propertyAddressesOut.AddAsync(propertyAddress);
        }
    }
}
