using FluentAssertions;
using FluentAssertions.Extensions;
using NewsparkWiremockDotNetDeepdive.Helpers;
using NewsparkWiremockDotNetDeepdive.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace NewsparkWiremockDotNetDeepdive.Answers
{
    public class Answers07pro : TestBase
    {
        private DebugTools _debugTools;

        public Answers07pro()
        {
            _debugTools = new DebugTools();
        }

        /************************************************
         * Create a stub that listens to:
         * 
         * 1) POST 	{hostname}/api/workitem
         * will return 201 + header location = "{hostname}/api/workitem/{workitemId}"
         * where workitemId will be a random generated guid (as the real application,
         * will also create this new unique id)
         * so the header returned wil be the actual url of the below mentioned get call, 
         * which will return the just created workitem
         * 
         * as input this POST stub will receive a json body with the following 3 keys 
         *      - status(string) 
         *      - assigned(boolean) 
         *      - deadline(datetime) 
         * but remember for test suport it is not always need to validate ont his, 
         * and when not needed, do not implement ;-) Keep your stub stupid an simple     
         *
         * Implement below stubs in a new method, as they all require the 'workitemId' 
         * from the location header as input:
         * 
         * 2) GET  {hostname}/api/workitem/{workitemId}
         * wil return 200 + a json body as specified in the 'models' folder 
         * as 'ResponseBodySimpleGet.cs'
         * 
         * 3) GET  {hostname}/api/workitem/{workitemId}/subtasks?status={status}
         * wil return 200 + a json body as specified in the 'models' folder
         * as 'ResponseBodyGetFilter.cs' (without 'FirstWorkItemId')
         * 
         * 4) POST {hostname}/api/workitems/filter
         * wil return 200 + a json body as specified in the 'models' folder
         * as 'ResponseBodyGetFilter.cs' (with 'FirstWorkItemId')
         * As input this stub expect a json body with status 'new'
         * some context: often you will see that post calls will be used as 
         * kind of get calls when the aplication want to search/filter for more 
         * than 2 or 3 query params as the get calls have no posibility to add 
         * a json body for aditional input parameters.
         * 
         * Add 2 -4:
         * Should return data from initial postcall when post was done succesfully, 
         * otherwise it will return 404.
         ************************************************/

        private void SetupStubInitialPost()
        {
            server.Given(
                Request.Create()
                    .WithPath("/api/workitem")
                .UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(201)
                    .WithHeader( "Location", "{{request.url}}" + $"/{Guid.NewGuid()}").WithTransformer()
            );
        }

        private void SetupStubFollowUpRequests(Guid workitemId) 
        {
            var responseBodySimpleGet = new
            {
                WorkitemId = workitemId,
                Assigned = false,
                Status = "new",
                Deadline = DateTime.UtcNow
            };

            // get specific workitem
            server.Given(
                Request.Create()
                    .WithPath($"/api/workitem/{workitemId}")
                .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(responseBodySimpleGet)
            );

            var responseBodyFilter = new
            {
                ResultsFound = 1,
                Data = responseBodySimpleGet
            };

            // get individual workitem based on query params
            server.Given(
                Request.Create()
                    .WithPath($"/api/workitem/{workitemId}/subtasks")
                    .WithParam("status", WireMock.Matchers.MatchBehaviour.AcceptOnMatch, "new")
                .UsingGet()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(responseBodyFilter)
            );

            var responseBodyComplexFilter = new
            {
                ResultsFound = 1,
                FirstWorkItemId = workitemId,
                Data = responseBodySimpleGet
            };

            // get workitem based on (complex) filter
            server.Given(
                Request.Create()
                    .WithPath($"/api/workitems/filter")
                    .WithBody(new JmesPathMatcher("status == 'new'"))
                .UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(responseBodyComplexFilter)
            );
        }

        [Test]
        public async Task TestExercise701()
        {
            // initialize first stub
            SetupStubInitialPost();

            // first request:
            RestRequest request1 = new RestRequest("/api/workitem", Method.Post);
            request1.AddJsonBody(new RequestBodySimplePost());
            RestResponse response1 = await client.ExecuteAsync(request1);

            response1.StatusCode.Should().Be(HttpStatusCode.Created);

            // validate header:
            var returnedLocationHeader = response1.Headers.FirstOrDefault(x => x.Name == "Location");
            returnedLocationHeader.Should().NotBeNull();
            returnedLocationHeader.Value.ToString().Should().Contain(response1.ResponseUri.ToString());

            // validate required body elements:
            var logRequests = server.FindLogEntries(
                Request.Create().WithPath("/api/workitem").UsingPost()
            );

            logRequests.Should().HaveCount(1);
            var logRequestThatContainedTheBody = logRequests.Single(x => x.RequestMessage != null);
            logRequestThatContainedTheBody.Should().NotBeNull();

            var bodyAsString = logRequestThatContainedTheBody.RequestMessage.Body;
            ResponseBodySimpleGet bodyAsJson = JsonConvert.DeserializeObject<ResponseBodySimpleGet>(bodyAsString);
            bodyAsJson.WorkitemId.Should().NotBeEmpty();

            // get guid for follow up call
            var justCreatedUniqueWorkItemId = returnedLocationHeader.Value.ToString().Substring(returnedLocationHeader.Value.ToString().LastIndexOf('/') + 1);

            // initialize second stub
            SetupStubFollowUpRequests(Guid.Parse(justCreatedUniqueWorkItemId));

            // second request:
            RestRequest request2 = new RestRequest($"/api/workitem/{justCreatedUniqueWorkItemId}", Method.Get);
            RestResponse<ResponseBodySimpleGet> response2 = await client.ExecuteAsync<ResponseBodySimpleGet>(request2);

            response2.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.Data.WorkitemId.Should().Be(justCreatedUniqueWorkItemId);
            response2.Data.Deadline.Should().BeCloseTo(DateTime.UtcNow, 300.Milliseconds());
            response2.Data.Status.Should().Be("new");
            response2.Data.Assigned.Should().NotBe(true);

            // third request:
            RestRequest request3 = new RestRequest($"/api/workitem/{justCreatedUniqueWorkItemId}/subtasks", Method.Get);
            request3.AddQueryParameter("status", "new");
            RestResponse<ResponseBodyGetFilter> response3 = await client.ExecuteAsync<ResponseBodyGetFilter>(request3);

            response3.StatusCode.Should().Be(HttpStatusCode.OK);
            response3.Data.ResultsFound.Should().Be(1);

            // some code for debuging / understanding what hapens:
            _debugTools.showAndOrValidateNumberOfReceivedRequests(server, "/api/workitem*", Method.Get, 2);

            // fourth request:
            RestRequest request4 = new RestRequest($"/api/workitems/filter", Method.Post);
            request4.AddJsonBody(new
            {
                status = "new"
            });
            RestResponse<ResponseBodyGetFilter> response4 = await client.ExecuteAsync<ResponseBodyGetFilter>(request4);

            // some code for debuging / understanding what hapens:
            _debugTools.showAndOrValidateNumberOfReceivedRequests(server, "/api/workitems/filter", Method.Post, 1);

            response4.StatusCode.Should().Be(HttpStatusCode.OK);
            response4.Data.ResultsFound.Should().Be(1);
            response4.Data.FirstWorkItemId.Should().Be(justCreatedUniqueWorkItemId);
        }
    }
}
