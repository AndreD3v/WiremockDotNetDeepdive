using System;

namespace NewsparkWiremockDotNetDeepdive.Models
{
    public class RequestBodySimplePost
    {
        public bool Assigned { get; set; } = false;

        public string Status { get; set; } = "dummy from model";

        public DateTime Deadline { get; set; } = DateTime.UtcNow.AddDays(-2).AddHours(7);
    }
}
