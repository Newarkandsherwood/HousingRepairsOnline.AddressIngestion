using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using HousingRepairsOnline.AddressIngestion.Services;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using Xunit;

namespace HousingRepairsOnline.AddressIngestion.Tests.Services;

public class RecreateDocumentCollectionTests
{
    private readonly Mock<IDocumentClient> _documentClientMock;
    private readonly RecreateDocumentCollection _recreateDocumentCollection;

    public RecreateDocumentCollectionTests()
    {
        _documentClientMock = new Mock<IDocumentClient>();
        _recreateDocumentCollection = new RecreateDocumentCollection(_documentClientMock.Object);
    }
    [Fact]
    public async Task GivenRecreateDocumentCollectionThenDeleteDocumentCollectionAsyncIsCalled()
    {
        await _recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), "collectionName", "partitionKey");
        _documentClientMock.Verify(x => x.DeleteDocumentCollectionAsync(It.IsAny<Uri>(), It.IsAny<RequestOptions>()), Times.Once);
    }
    [Fact]
    public async Task GivenRecreateDocumentCollectionThenCreateDocumentCollectionAsyncIsCalled()
    {
        var databaseUri = new Uri("http://www.google.com/");

        await _recreateDocumentCollection.Execute(databaseUri, It.IsAny<Uri>(), "collectionName", "partitionKey");
        _documentClientMock.Verify(x => x.CreateDocumentCollectionAsync(databaseUri, It.IsAny<DocumentCollection>(), null), Times.Once);
    }

    [Fact]
    public async Task GiveNoCollectionNameThenUriFormatExceptionIsThrown()
    {

        Func<Task> act = () => _recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), null, It.IsAny<string>());

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
    }
    [Fact]
    public async Task GiveEmptyCollectionNameThenUriFormatExceptionIsThrown()
    {

        Func<Task> act = () => _recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), "", It.IsAny<string>());

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
    }
    [Fact]
    public async Task GiveNoPartitionKeyThenUriFormatExceptionIsThrown()
    {

        Func<Task> act = () => _recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<string>(), null);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
    }
    [Fact]
    public async Task GiveEmptyPartitionKeyThenUriFormatExceptionIsThrown()
    {

        Func<Task> act = () => _recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<string>(), "");

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
    }
}