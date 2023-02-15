using FluentAssertions;
using NewsparkWiremockDotNetDeepdive.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace NewsparkWiremockDotNetDeepdive.SwaggerPetStoreExamples
{
    public class SwaggerPetStoreMock
    {
        private WireMockServer _server;
        private CreateDummyTestData _testData;

        public SwaggerPetStoreMock()
        {
            _testData = new CreateDummyTestData();
        }

        [SetUp]
        public void StartServer()
        {
            _server = WireMockServer.Start(9876);
        }

        public void ConfigureMockingService()
        {
            var request = Request.Create()
                .WithPath("/pet/findByStatus")
                .WithParam("status", WireMock.Matchers.MatchBehaviour.AcceptOnMatch, "pending")
                .UsingGet();

            var response = Response.Create()
                .WithStatusCode(200)
                .WithBodyAsJson(_testData.GenerateDummyData(2));

            _server.Given(request)
                .RespondWith(response);
        }

        //Warning: before running test (against real endpoint) replace
        //the second value (representing 'expectedNameFirstPet') below,
        //this value will change over time as lot of people use this
        //endoint. the fact that testdata isn't stable directly
        //demonstrates the added value of having mocks available
        [TestCase("https://petstore.swagger.io/v2", "Fruit", TestName = "Real service")]
        [TestCase("http://localhost:9876", "doggie 0", TestName = "Mock service")]
        public async Task TestCompareActualServiceWithMock(string baseUrl, string expectedNameFirstPet)
        {
            ConfigureMockingService();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}/pet/findByStatus?status=pending");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var pets = JsonConvert.DeserializeObject<List<Pet>>(jsonString);

                    for (int i = 0; i < 1; i++)
                    {
                        Console.WriteLine($"Name of pet from {baseUrl}: {pets[i].Name}");
                    }
                    pets.First().Name.Should().Be(expectedNameFirstPet);
                }
                response.IsSuccessStatusCode.Should().BeTrue();
            }
        }

        [TearDown]
        public void StopServer()
        {
            _server.Stop();
        }
    }
}
