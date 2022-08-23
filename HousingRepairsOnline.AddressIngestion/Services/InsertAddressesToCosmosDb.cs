using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Azure.WebJobs;
using HACT.Dtos;

namespace HousingRepairsOnline.AddressIngestion.Services;

using System.Linq;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Logging;

public class InsertAddressesToCosmosDb
{
    private readonly IAsyncCollector<PropertyAddress> propertyAddressesOut;
    private readonly ILogger logger;

    public InsertAddressesToCosmosDb(IAsyncCollector<PropertyAddress> propertyAddressesOut, ILogger logger)
    {
        this.propertyAddressesOut = propertyAddressesOut;
        this.logger = logger;
    }

    public async Task Execute(IEnumerable<PropertyAddress> propertyAddresses )
    {
        foreach (var propertyAddress in propertyAddresses)
        {
            if (propertyAddress.AddressLine.FirstOrDefault() == "" || propertyAddress.AddressLine.FirstOrDefault() == null )
            {
                logger.LogInformation($"AddressLine for property {propertyAddress.Reference.ID} is null or empty");
            }
            else if (string.IsNullOrEmpty(propertyAddress.PostalCode) )
            {
                logger.LogInformation($"Postalcode for property {propertyAddress.PostalCode} is null or empty");
            }
            else if (string.IsNullOrEmpty(propertyAddress.Reference.ID) )
            {
                logger.LogInformation($"Property Id is null or empty");
            }
            else
            {
                await propertyAddressesOut.AddAsync(propertyAddress);
            }
        }
    }


}
