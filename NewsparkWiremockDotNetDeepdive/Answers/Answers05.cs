using FluentAssertions;
using NUnit.Framework;
using RestSharp;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace NewsparkWiremockDotNetDeepdive.Answers
{
    public class Answers05 : TestBase
    {
        private void SetupStubServiceUnavailable()
        {

            server.Given(
                Request.Create()
                    .WithPath("/requestLoan")
                .UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(503)
            );
        }

        [Test]
        public async Task TestExercise501()
        {
            SetupStubServiceUnavailable();

            // first request:
            RestRequest request = new RestRequest("/requestLoan", Method.Post);
            RestResponse response = await client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

            // second request:
            RestRequest request2 = new RestRequest("/requestLoan", Method.Post);
            request2.AddBody("visit newspark.nl");
            RestResponse response2 = await client.ExecuteAsync(request2);

            /**
             * Add a verification to this test that verifies that exactly two HTTP POST
             * has been submitted to the /requestLoan endpoint
             * 
             * bonus chalange: Add a verification to this test that verifies that one of 
             * the two HTTP POST calls did contain a body with the string "visit newspark.nl"
             * has been submitted to the /requestLoan endpoint
             */

            // posible answer initial exercise:
            var logRequests = server.FindLogEntries(
                Request.Create().WithPath("/requestLoan").UsingPost()
            );
                        
            logRequests.Should().HaveCount(2);

            // posible answer bonus chalange:
            var logRequestThatContainedTheBody = logRequests.Single(x => x.RequestMessage.Body == "visit newspark.nl");
            logRequestThatContainedTheBody.Should().NotBeNull();
            logRequestThatContainedTheBody.RequestMessage.Body.Should().Be("visit newspark.nl");
        }
    }
}
