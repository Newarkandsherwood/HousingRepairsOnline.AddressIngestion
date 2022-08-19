using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using HousingRepairsOnline.AddressIngestion.Domain;

namespace HousingRepairsOnline.AddressIngestion.Helpers
{
    public class CSVInputStreamHelperTests
    {
        [Fact]
        public void GivenACSVStreamWithAddressesWhenMapTOCommunalAddressesThenAllRecordsMapped()
        {
            var csvTextFile =
                @"PLACE_REF,Textbox4,POST_CODE
            1009999,""Block: Abbey 1 (flats 18-24), Abbey Road, Edwinstown"",XX21 9LQ
            8800547,""Block: Abbey 2 (flats 38-44), Abbey Road, Edwinstown"",XX21 9LQ
            8806094,""Block: Aldern Grove (flats 20-28), Alder Grove, Ollerby"",XX22 9UB";

            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(csvTextFile)))
            {
                var result = CsvInputStreamHelper.MapToCommunalAddresses(test_Stream);
                Assert.True(result.ToList().Count() == 3);
            }
        }

        [Fact]
        public void GivenACSVStreamWithAddressesWhenMapToCommunalAddressesThenTheyAreMappedToCommunalAddresses()
        {
            var csvTextFile =
                @"PLACE_REF,Textbox4,POST_CODE
            1009999,""Block: Abbey 1 (flats 18-24), Abbey Road, Edwinstown"",XX21 9LQ
            8800547,""Block: Abbey 2 (flats 38-44), Abbey Road, Edwinstown"",XX21 9LQ
            8806094,""Block: Aldern Grove (flats 20-28), Alder Grove, Ollerby"",XX22 9UB";

            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(csvTextFile)))
            {
                var result = CsvInputStreamHelper.MapToCommunalAddresses(test_Stream);
                Assert.True(result.FirstOrDefault()?.GetType() == typeof(CommunalAddress));
            }
        }
    }
}