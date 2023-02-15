﻿using FluentAssertions;
using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Threading.Tasks;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace NewsparkWiremockDotNetDeepdive.Exercises
{
    public class Exercises03 : TestBase
    {
        public void SetupStubExercise301()
        {

            /************************************************
             * Create a stub that exerts the following behavior:
             * - The scenario is called 'Loan processing'
             * - 1. A first GET to /loan/12345 returns HTTP 404
             * - 2. A POST to /requestLoan with body 'Loan ID: 12345' returns HTTP 201
             * 		and causes a transition to state 'LOAN_GRANTED'
             * - 3. A second GET (when in state 'LOAN_GRANTED') to /loan/12345
             *      returns HTTP 200 and body 'Loan ID: 12345'
             ************************************************/


        }

        [Test]
        public async Task TestExercise301()
        {
            SetupStubExercise301();

            //initial get:
            RestRequest getRequest1 = new RestRequest("/loan/12345", Method.Get);

            RestResponse getResponse1 = await client.ExecuteAsync(getRequest1);
            getResponse1.StatusCode.Should().Be(HttpStatusCode.NotFound);

            //post:
            RestRequest postRequest = new RestRequest("/requestLoan", Method.Post);
            postRequest.AddBody("Loan ID: 12345");

            RestResponse postResponse = await client.ExecuteAsync(postRequest);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            //second get:
            RestRequest getRequest2 = new RestRequest("/loan/12345", Method.Get);

            RestResponse getResponse2 = await client.ExecuteAsync(getRequest2);
            getResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse2.Content.Should().Be("Loan ID: 12345");
        }
    }
}
