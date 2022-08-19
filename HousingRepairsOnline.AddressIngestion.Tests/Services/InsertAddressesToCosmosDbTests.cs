using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HousingRepairsOnline.AddressIngestion.Domain;
using HousingRepairsOnline.AddressIngestion.Services;
using Microsoft.Azure.WebJobs;
using Moq;
using Xunit;

namespace HousingRepairsOnline.AddressIngestion.Tests.Services;

public class InsertAddressesToCosmosDBTests
{
    private readonly Mock<IAsyncCollector<CommunalAddress>> _collectorMock;
    private readonly InsertAddressesToCosmosDb _insertAddessesToCosmosDB;

    public InsertAddressesToCosmosDBTests()
    {
        _collectorMock = new Mock<IAsyncCollector<CommunalAddress>>();
        _insertAddessesToCosmosDB = new InsertAddressesToCosmosDb(_collectorMock.Object);
    }
    [Fact]
    public async Task GivenCommunalAddressesWhenInsertAddressesToCosmosDBThenAddAsyncIsCalled()
    {
        var mockCommunalAddress = new CommunalAddress
        {
            AddressLine = "AddressLine",
            PlaceReference = 1,
            PostCode = "AddressLine"
        };
        var addresses = new List<CommunalAddress> { mockCommunalAddress };
        await _insertAddessesToCosmosDB.Execute(addresses);
        _collectorMock.Verify(x => x.AddAsync(mockCommunalAddress, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
    [Fact]
    public async Task GivenEmptyCommunalAddressesWhenInsertAddressesToCosmosDBThenAddAsyncIsCalled()
    {
        var mockCommunalAddress = new CommunalAddress();
        var addresses = new List<CommunalAddress> { mockCommunalAddress };

        Func<Task> act = () => _insertAddessesToCosmosDB.Execute(addresses);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
    }
    [Fact]
    public async Task GivenThreeCommunalAddressesWhenInsertAddressesToCosmosDBThenAddAsyncIsCalledThreeTimes()
    {
        var mockCommunalAddress1 = new CommunalAddress { AddressLine = "AddressLine", PlaceReference = 1, PostCode = "AddressLine" };
        var mockCommunalAddress2 = new CommunalAddress { AddressLine = "AddressLin2", PlaceReference = 2, PostCode = "AddressLine2" };
        var mockCommunalAddress3 = new CommunalAddress { AddressLine = "AddressLine3", PlaceReference = 3, PostCode = "AddressLine3" };

        var addresses = new List<CommunalAddress> { mockCommunalAddress1, mockCommunalAddress2, mockCommunalAddress3 };
        await _insertAddessesToCosmosDB.Execute(addresses);
        _collectorMock.Verify(x => x.AddAsync(It.IsAny<CommunalAddress>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }
}