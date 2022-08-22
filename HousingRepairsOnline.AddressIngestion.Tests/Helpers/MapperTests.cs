using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using System.Collections.Generic;


namespace HousingRepairsOnline.AddressIngestion.Helpers
{
    using Domain;
    using FluentAssertions;

    public class MapperTests
    {
        [Fact]
        public void GivenACSVStreamWithAddressesWhenMapToAddressesThenAllRecordsMapped()
        {
            var csvTextFile =
                @"PLACE_REF,Textbox4,POST_CODE
            1009999,""Block: Abbey 1 (flats 18-24), Abbey Road, Edwinstown"",XX21 9LQ
            8800547,""Block: Abbey 2 (flats 38-44), Abbey Road, Edwinstown"",XX21 9LQ
            8806094,""Block: Aldern Grove (flats 20-28), Alder Grove, Ollerby"",XX22 9UB";

            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(csvTextFile)))
            {
                var result = Mapper.CsvInputStreamToAddresses(test_Stream);
                Assert.True(result.ToList().Count() == 3);
            }
        }

        [Fact]
        public void GivenACSVStreamWithAddressesWhenMapToAddressesThenTheyAreMappedToAddresses()
        {
            var csvTextFile =
                @"PLACE_REF,Textbox4,POST_CODE
            1009999,""Block: Abbey 1 (flats 18-24), Abbey Road, Edwinstown"",XX21 9LQ
            8800547,""Block: Abbey 2 (flats 38-44), Abbey Road, Edwinstown"",XX21 9LQ
            8806094,""Block: Aldern Grove (flats 20-28), Alder Grove, Ollerby"",XX22 9UB";

            using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes(csvTextFile)))
            {
                var result = Mapper.CsvInputStreamToAddresses(test_Stream);
                Assert.True(result.FirstOrDefault()?.GetType() == typeof(Address));
            }
        }

        [Fact]
        public void GiveAnAddressWhenToHactPropertyAddressesThenItIsMappedToAPropertyAddress()
        {
            var mockAddress1 = new Address { AddressLine = "AddressLine" , PlaceReference =  1, PostCode = "AddressLine" };
            var mockAddress2 = new Address { AddressLine = "AddressLin2", PlaceReference = 2, PostCode = "AddressLine2" };

            var addresses = new List<Address> { mockAddress1, mockAddress2 };
            var propertyAddresses = Mapper.ToHactPropertyAddresses(addresses);
            propertyAddresses.FirstOrDefault().AddressLine.Should().Equal(mockAddress1.AddressLine);
            propertyAddresses.FirstOrDefault().Reference.ID.Should().BeEquivalentTo(mockAddress1.PlaceReference.ToString());
            propertyAddresses.FirstOrDefault().PostalCode.Should().BeEquivalentTo(mockAddress1.PostCode);
            propertyAddresses.FirstOrDefault().Reference.AllocatedBy.Should().BeEquivalentTo("Capita");

        }
    }
}
