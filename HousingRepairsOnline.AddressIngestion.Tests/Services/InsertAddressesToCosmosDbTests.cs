using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HousingRepairsOnline.AddressIngestion.Services;
using Microsoft.Azure.WebJobs;
using Moq;
using Xunit;
using HACT.Dtos;


namespace HousingRepairsOnline.AddressIngestion.Tests.Services;

public class InsertAddressesToCosmosDBTests
{
    private readonly Mock<IAsyncCollector<PropertyAddress>> _collectorMock;
    private readonly InsertAddressesToCosmosDb _insertAddessesToCosmosDB;

    public InsertAddressesToCosmosDBTests()
    {
        _collectorMock = new Mock<IAsyncCollector<PropertyAddress>>();
        _insertAddessesToCosmosDB = new InsertAddressesToCosmosDb(_collectorMock.Object);
    }
    [Fact]
    public async Task GivenAddressesWhenInsertAddressesToCosmosDBThenAddAsyncIsCalled()
    {
        var mockAddress = new PropertyAddress
        {
            AddressLine = new[] { "AddressLine" },
            Reference = new Reference { ID = "1", Description = "Housing repairs" },
            PostalCode = "AddressLine"
        };
        var addresses = new List<PropertyAddress> { mockAddress };
        await _insertAddessesToCosmosDB.Execute(addresses);
        _collectorMock.Verify(x => x.AddAsync(mockAddress, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
    [Fact]
    public async Task GivenEmptyAddressesWhenInsertAddressesToCosmosDBThenAddAsyncIsCalled()
    {
        var mockAddress = new PropertyAddress();
        var addresses = new List<PropertyAddress> { mockAddress };

        Func<Task> act = () => _insertAddessesToCosmosDB.Execute(addresses);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
    }
    [Fact]
    public async Task GivenThreeAddressesWhenInsertAddressesToCosmosDBThenAddAsyncIsCalledThreeTimes()
    {
        var mockAddress1 = new PropertyAddress { AddressLine = new []{"AddressLine"} , Reference = new Reference { ID = "1", Description = "Housing repairs" }, PostalCode = "AddressLine" };
        var mockAddress2 = new PropertyAddress { AddressLine = new []{"AddressLin2"}, Reference = new Reference { ID = "2", Description = "Housing repairs" }, PostalCode = "AddressLine2" };
        var mockAddress3 = new PropertyAddress { AddressLine = new []{"AddressLine3"}, Reference = new Reference { ID = "3", Description = "Housing repairs" }, PostalCode = "AddressLine3" };

        var addresses = new List<PropertyAddress> { mockAddress1, mockAddress2, mockAddress3 };
        await _insertAddessesToCosmosDB.Execute(addresses);
        _collectorMock.Verify(x => x.AddAsync(It.IsAny<PropertyAddress>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _collectorMock.Verify(x => x.AddAsync(mockAddress1, It.IsAny<CancellationToken>()), Times.Exactly(1));
        _collectorMock.Verify(x => x.AddAsync(mockAddress2, It.IsAny<CancellationToken>()), Times.Exactly(1));
        _collectorMock.Verify(x => x.AddAsync(mockAddress3, It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}
