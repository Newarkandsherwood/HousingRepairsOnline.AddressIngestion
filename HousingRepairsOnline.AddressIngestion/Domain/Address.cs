namespace HousingRepairsOnline.AddressIngestion.Domain
{
    using CsvHelper.Configuration.Attributes;

    public class Address
    {
        [Index(0)]
        public int PlaceReference { get; set; }
        [Index(1)]
        public string AddressLine { get; set; }
        [Index(2)]
        public string PostCode { get; set; }
    }
}
