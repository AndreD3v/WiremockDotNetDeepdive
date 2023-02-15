using FluentAssertions;
using NewsparkWiremockDotNetDeepdive.Models;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace NewsparkWiremockDotNetDeepdive.Exercises
{
    public class Exercises04 : TestBase
    {
        public void SetupStubExercise401()
        {

            /************************************************
             * Create a stub that responds to all GET requests
             * to /echo-port with HTTP status code 200 and a
             * response body containing the text
             * "Listening on port <portnumber>", where <portnumber>
             * is replaced with the actual port number
             * (9876, in this case) using response templating.
             *
             * TIP 1: https://github.com/WireMock-Net/WireMock.Net/wiki/Response-Templating
             ************************************************/


        }

        public void SetupStubExercise402()
        {

            /************************************************
             * Create a stub that listens at path /echo-loan-amount
             * and responds to all POST requests with HTTP
             * status code 201 and a response body containing
             * the text 'Received loan application request for $<amount>',         *
             * where <amount> is the value of the JSON element
             * loanDetails.amount extracted from the request body
             ************************************************/


        }

        [Test]
        public async Task TestStubExercise401()
        {
            SetupStubExercise401();

            RestRequest request = new RestRequest("/echo-port", Method.Get);

            RestResponse response = await client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().Be("Listening on port 9876");
        }

        [TestCase(1000, TestName = "TestStubExercise402 Loan $1000")]
        [TestCase(1500, TestName = "TestStubExercise402 Loan $1500")]
        [TestCase(3000, TestName = "TestStubExercise402 Loan $3000")]
        public async Task TestStubExercise402(int loanAmount)
        {
            SetupStubExercise402();

            var loanDetails = new LoanDetails();
            loanDetails.Amount = loanAmount;

            var loanRequest = new LoanRequest();
            loanRequest.LoanDetails = loanDetails;

            RestRequest request = new RestRequest("/echo-loan-amount", Method.Post);
            request.AddBody(loanRequest);

            RestResponse response = await client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content.Should().Be($"Received loan application request for ${loanAmount}");
        }
    }
}
