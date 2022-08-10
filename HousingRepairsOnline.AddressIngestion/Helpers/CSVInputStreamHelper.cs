using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using HousingRepairsOnline.AddressIngestion.Domain;

namespace HousingRepairsOnline.AddressIngestion.Helpers;

public class CSVInputStreamHelper
{
    public static IEnumerable<CommunalAddress> MapToCommunalAddresses(Stream inputStream)
    {
        using var reader = new StreamReader(inputStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<CommunalAddress>().ToList();
    }
}