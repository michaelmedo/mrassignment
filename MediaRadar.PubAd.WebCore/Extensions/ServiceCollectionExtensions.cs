
using MediaRadar.API.SDK;
using MediaRadar.PubAd.WebCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaRadar.PubAd.WebCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMeidaRadarServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            MediaRadarSettings config = new MediaRadarSettings();
            configuration.GetSection("MediaRadar").Bind(config);

            services.AddTransient<IPublicationAdsAPI, PublicationAdsAPI>
                (s => new PublicationAdsAPI(config.URL, config.SubscriptionKey, null));

            return services;
        }
    }
}
