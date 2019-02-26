using System.Net;

namespace MediaRadar.API.SDK
{
    public class RequestResult
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string Content { get; set; }
    }
}