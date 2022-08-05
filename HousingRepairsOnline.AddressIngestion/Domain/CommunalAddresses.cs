namespace HousingRepairsOnline.AddressIngestion.Domain
{
    public class CommunalAddresses
    {
        public string Id { get; set;}
        public int PlaceReference { get; set; }
        public string AddressLine { get; set; }
        public string PostCode { get; set; }
    }
}