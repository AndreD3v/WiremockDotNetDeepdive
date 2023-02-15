using FluentAssertions;
using RestSharp;
using System;
using System.Collections.Generic;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.Server;

namespace NewsparkWiremockDotNetDeepdive.Helpers
{
    public class DebugTools
    {
        public IEnumerable<LogEntry> showAndOrValidateNumberOfReceivedRequests(WireMockServer server, string path, Method restMethod, int expectedNumberOfReceivedRequests)
        {
            IEnumerable<LogEntry> logRequests = new List<LogEntry>();
            switch (restMethod)
            {
                case Method.Post :
                    logRequests = server.FindLogEntries(Request.Create().WithPath(path).UsingPost());
                    break;
                case Method.Get :
                    logRequests = server.FindLogEntries(Request.Create().WithPath(path).UsingGet());
                    break;
                default :
                    throw new NotImplementedException($"Rest method '{restMethod}' not added to switch case yet.");
            }

            logRequests.Should().HaveCount(expectedNumberOfReceivedRequests, $"we used Url path: {path} and REST method {restMethod}");

            return logRequests; 
        }
    }
}
