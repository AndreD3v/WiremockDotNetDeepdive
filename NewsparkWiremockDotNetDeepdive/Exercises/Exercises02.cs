using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace NewsparkWiremockDotNetDeepdive.Exercises
{
    public class Exercises02 : TestBase
    {

        public void SetupStubExercise201()
        {
            /************************************************
             * Create a stub that will respond to all POST
             * requests to /requestLoan with HTTP status code 503 
             ************************************************/


        }

        public void SetupStubExercise202()
        {
            /************************************************
             * Create a stub that will respond to a POST request
             * to /requestLoan, but only if this request contains
             * a header 'speed' with value 'slow'.
             *
             * Respond with status code 200, but only after a
             * fixed delay of 3000 milliseconds.
             ************************************************/


        }

        public void SetupStubExercise203()
        {

            /************************************************
             * Create a stub that will respond to a POST request
             * to /requestLoan, but only if this request contains
             * a cookie 'session' with value 'invalid'
             *
             * Respond with a Fault of type MALFORMED_RESPONSE_CHUNK
             ************************************************/


        }

        private void SetupStubExercise204a()
        {
            /************************************************
		     * Create a stub that will respond to a POST request
		     * to /requestLoan, but only if this request contains
		     * a JSON request body with an element 'status' with
		     * value 'active'.
		     * 
		     * Respond with an HTTP status code 201.
		     ************************************************/


        }

        //Warning: below exercises become more difficult

        public void SetupStubExercise204b()
        {

            /************************************************
             * Create a stub that will respond to a POST request
             * to /requestLoan with status code 200,
             * but only if:
             * - the 'backgroundCheck' header has value 'OK'
             * - the 'backgroundCheck' header is not present
             ************************************************/


        }

        public void SetupStubExercise205()
        {

            /************************************************
             * Create a stub that will respond to a POST request
             * to /requestLoan with status code 200,
             * but only if the loan amount specified in the
             * request body is equal to 1000.
             *
             * The loan amount is specified in the 'amount'
             * field, which is a child element of 'loanDetails'
             * 
             * TIP 1: https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching#two-matchers
             * Be aware code example from above url for JmesPathMatcher contains an almost invisible typo ;-) 
             * Tip 2: use JmesPathMatcher (https://jmespath.org/examples.html)
             * Tip 3: use if necessary https://jsfiddle.net/v72hkrst/ to test your JmesPathMatcher
             ************************************************/


        }

        [Test]
        public async Task TestStubExercise201()
        {
            SetupStubExercise201();

            RestRequest request = new RestRequest("/requestLoan", Method.Post);

            RestResponse response = await client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            response.StatusDescription.Should().BeOneOf("Loan processor service unavailable", "Service Unavailable");
        }

        [Test]
        public async Task TestStubExercise202()
        {
            SetupStubExercise202();

            RestRequest request = new RestRequest("/requestLoan", Method.Post);
            request.AddHeader("speed", "slow");

            var watcher = Stopwatch.StartNew();
            RestResponse response = await client.ExecuteAsync(request);
            watcher.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            watcher.ElapsedMilliseconds.Should().BeGreaterThan(3000).And.BeLessThan(4000);

            RestRequest requestWithoutHeader = new RestRequest("/requestLoan", Method.Post);
            RestResponse responseSecondRequest = await client.ExecuteAsync(requestWithoutHeader);
            responseSecondRequest.StatusCode.Should().Be(HttpStatusCode.NotFound, "If you see this error message you know you have implemented the time requirement well, but missed something regarding the header ;-)");
        }

        [Test]
        public async Task TestStubExercise203()
        {
            SetupStubExercise203();

            RestRequest request = new RestRequest("/requestLoan", Method.Post);

            client.CookieContainer.Add(new Cookie("session", "invalid", "/requestLoan", "localhost"));

            RestResponse response = await client.ExecuteAsync(request);

            Action act = () => JObject.Parse(response.Content);
            act.Should().Throw<JsonReaderException>();
        }

        [Test]
        public async Task TestStubExercise204a()
        {
            SetupStubExercise204a();

            RestRequest request = new RestRequest("/requestLoan", Method.Post);

            request.AddJsonBody(new { status = "active" });

            RestResponse response = await client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task TestStubExercise204b()
        {
            SetupStubExercise204b();

            //situation 1:
            RestRequest request1 = new RestRequest("/requestLoan", Method.Post);
            request1.AddHeader("backgroundCheck", "OK");

            RestResponse response1 = await client.ExecuteAsync(request1);

            response1.StatusCode.Should().Be(HttpStatusCode.OK);

            //situation 2:
            RestRequest request2 = new RestRequest("/requestLoan", Method.Post);
            request2.AddHeader("backgroundCheck", "");

            RestResponse response2 = await client.ExecuteAsync(request2);

            response2.StatusCode.Should().Be(HttpStatusCode.OK);

            //situation 3:
            RestRequest request3 = new RestRequest("/requestLoan", Method.Post);
            request3.AddHeader("backgroundCheck", "FAILED");

            RestResponse response3 = await client.ExecuteAsync(request3);

            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);

            //situation 4: No header backgroundCheck
            RestRequest request4 = new RestRequest("/requestLoan", Method.Post);
            RestResponse response4 = await client.ExecuteAsync(request4);
            response4.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestCase(1000, HttpStatusCode.OK, TestName = "TestStubExercise205 Loan $1000")]
        [TestCase(1500, HttpStatusCode.NotFound, TestName = "TestStubExercise205 Loan $1500")]
        public async Task TestStubExercise205(int loanAmount, HttpStatusCode httpStatusCode)
        {
            SetupStubExercise205();

            var loanRequest = new
            {
                loanDetails = new
                {
                    amount = loanAmount
                }
            };

            RestRequest request = new RestRequest("/requestLoan", Method.Post);
            request.AddJsonBody(loanRequest);
            
            RestResponse response = await client.ExecuteAsync(request);
            response.StatusCode.Should().Be(httpStatusCode);
        }
    }
}