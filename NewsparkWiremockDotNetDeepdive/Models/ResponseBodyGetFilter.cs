using System;

namespace NewsparkWiremockDotNetDeepdive.Models
{
    public class ResponseBodyGetFilter
    {
        public int ResultsFound { get; set; }

        public Guid FirstWorkItemId { get; set; }

        public ResponseBodySimpleGet Data { get; set; }
    }
}
