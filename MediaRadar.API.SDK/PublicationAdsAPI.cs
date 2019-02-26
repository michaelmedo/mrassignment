using MediaRadar.API.SDK.Requests;
using System;
using System.Net;

namespace MediaRadar.API.SDK
{
    public interface IPublicationAdsAPI
    {
        string ServiceUrl { get; }

        IPubAdActivities PubAdActivities { get; }
    }

    public class PublicationAdsAPI : IPublicationAdsAPI
    {
        private readonly string _serviceUrl = "";

        public string ServiceUrl => _serviceUrl;

        public IPubAdActivities PubAdActivities
        {
            get;
            set;
        }

        public PublicationAdsAPI(string serviceUrl, string ocp_SubscriptionKey, IWebProxy proxy)
        {
            _serviceUrl = GetFormattedUrl(serviceUrl).AbsoluteUri;
            PubAdActivities = new PubAdActivities(serviceUrl, ocp_SubscriptionKey, proxy);
        }

        private Uri GetFormattedUrl(string serviceUrl)
        {
            serviceUrl = serviceUrl.ToLower();
            if (serviceUrl.StartsWith("http://"))
            {
                serviceUrl = serviceUrl.Replace("http://", "https://");
            }

            if (!serviceUrl.StartsWith("https://"))
            {
                serviceUrl = "https://" + serviceUrl;
            }

            return new Uri(serviceUrl);
        }
    }
}
