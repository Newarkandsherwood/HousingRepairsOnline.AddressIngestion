namespace HousingRepairsOnline.AddressIngestion.Helpers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using HACT.Dtos;

    using Address = Domain.Address;

    public static class Mapper
    {
        public static IEnumerable<Address> CsvInputStreamToAddresses(Stream inputStream)
        {
            using var reader = new StreamReader(inputStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Address>().ToList();
        }

        public static IEnumerable<PropertyAddress> ToHactPropertyAddresses(IEnumerable<Address> addresses, string housingProvider) => (from address in addresses
                                                                                                                                       let propertyReference = address.PlaceReference == null
                                                                                                                                           ? null
                                                                                                                                           : new Reference { ID = address.PlaceReference.ToString(CultureInfo.InvariantCulture), AllocatedBy = housingProvider, }
                                                                                                                                       select new PropertyAddress { AddressLine = new[] { address.AddressLine }, PostalCode = address.PostCode, Reference = propertyReference }).ToList();
    }
}
