using System;
using Newtonsoft.Json;

namespace Weather.Logging
{
    public class Log
    {
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Method { get; set; }
        public string Payload { get; set; }
        public string Response { get; set; }
        public int? ResponseCode { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTime RespondedTime { get; set; }

        public override string ToString() => $"Request: {JsonConvert.SerializeObject(this)}";

    }
}