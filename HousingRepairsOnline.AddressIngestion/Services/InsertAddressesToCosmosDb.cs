namespace HousingRepairsOnline.AddressIngestion.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HACT.Dtos;
    using Microsoft.Azure.WebJobs;
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

        public async Task Execute(IEnumerable<PropertyAddress> propertyAddresses)
        {
            foreach (var propertyAddress in propertyAddresses)
            {
                if (propertyAddress.AddressLine.FirstOrDefault() is "" or null)
                {
                    this.logger.LogInformation($"AddressLine for property {propertyAddress.Reference.ID} is null or empty");
                }
                if (string.IsNullOrEmpty(propertyAddress.PostalCode))
                {
                    this.logger.LogInformation($"Postalcode for property {propertyAddress.PostalCode} is null or empty");
                }
                if (string.IsNullOrEmpty(propertyAddress.Reference.ID))
                {
                    this.logger.LogInformation($"Property Id is null or empty");
                }

                await this.propertyAddressesOut.AddAsync(propertyAddress);
            }
        }


    }
}
