namespace HousingRepairsOnline.AddressIngestion.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using HousingRepairsOnline.AddressIngestion.Services;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Moq;
    using Xunit;

    public class RecreateDocumentCollectionTests
    {
        private readonly Mock<IDocumentClient> _documentClientMock;
        private readonly RecreateDocumentCollection _recreateDocumentCollection;

        public RecreateDocumentCollectionTests()
        {
            this._documentClientMock = new Mock<IDocumentClient>();
            this._recreateDocumentCollection = new RecreateDocumentCollection(this._documentClientMock.Object);
        }
        [Fact]
        public async Task GivenRecreateDocumentCollectionThenDeleteDocumentCollectionAsyncIsCalled()
        {
            await this._recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), "collectionName", "partitionKey");
            this._documentClientMock.Verify(x => x.DeleteDocumentCollectionAsync(It.IsAny<Uri>(), It.IsAny<RequestOptions>()), Times.Once);
        }
        [Fact]
        public async Task GivenRecreateDocumentCollectionThenCreateDocumentCollectionAsyncIsCalled()
        {
            var databaseUri = new Uri("http://www.google.com/");

            await this._recreateDocumentCollection.Execute(databaseUri, It.IsAny<Uri>(), "collectionName", "partitionKey");
            this._documentClientMock.Verify(x => x.CreateDocumentCollectionAsync(databaseUri, It.IsAny<DocumentCollection>(), null), Times.Once);
        }

        [Fact]
        public async Task GiveNoCollectionNameThenUriFormatExceptionIsThrown()
        {

            Task act() => this._recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), null, It.IsAny<string>());

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
        }
        [Fact]
        public async Task GiveEmptyCollectionNameThenUriFormatExceptionIsThrown()
        {

            Task act() => this._recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), "", It.IsAny<string>());

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        }
        [Fact]
        public async Task GiveNoPartitionKeyThenUriFormatExceptionIsThrown()
        {

            Task act() => this._recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<string>(), null);

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
        }
        [Fact]
        public async Task GiveEmptyPartitionKeyThenUriFormatExceptionIsThrown()
        {

            Task act() => this._recreateDocumentCollection.Execute(It.IsAny<Uri>(), It.IsAny<Uri>(), It.IsAny<string>(), "");

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
        }
    }
}
