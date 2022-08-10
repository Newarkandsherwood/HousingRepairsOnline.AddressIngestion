using System;
using System.IO;
using Xunit;
using HousingRepairsOnline.AddressIngestion;
using HousingRepairsOnline.AddressIngestion.Domain;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;

namespace HousingRepairsOnline.AddressIngestion.xUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public async void GivenBlankInputs_WhenFunctionRuns_EnsureThatResultIsNotNull()
        {
            // required Environment Variables
            Environment.SetEnvironmentVariable("DatabaseName", "test");
            Environment.SetEnvironmentVariable("CollectionName", "test");
            Environment.SetEnvironmentVariable("PartitionKey", "test");
            Environment.SetEnvironmentVariable("CommunalBlobPath", "test");
            
            var timer = default(TimerInfo);
            var logger = Mock.Of<ILogger>();
            var blobStream = Mock.Of<Stream>();
            var documentClient = Mock.Of<IDocumentClient>();
            var communalAddressesOut = Mock.Of<IAsyncCollector<CommunalAddress>>(); 
            var result = await AddressIngestion.Function.RunAsync(timer, blobStream, communalAddressesOut, null, logger); 
            Assert.True(result != null);
        }

        // [Fact]
        // public async void GivenInputStreamIsNULL_WhenFunctionRuns_EnsureThatExceptionIsRaised()
        // {
        //     var timer = default(TimerInfo);
        //     var logger = Mock.Of<ILogger>();
        //     var blobStream = Mock.Of<Stream>();
        //     var documentClient = Mock.Of<IDocumentClient>();
        //     var communalAddressesOut = Mock.Of<IAsyncCollector<CommunalAddress>>(); 
        //     
        //     var result = AddressIngestion.Function.RunAsync(timer, null, communalAddressesOut, null, logger);
        //     //Assert.True(result != null);
        // }
        
        
    }
}