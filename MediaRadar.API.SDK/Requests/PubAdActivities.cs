using MediaRadar.API.SDK.Models.PubAdActivities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MediaRadar.API.SDK.Requests
{
    public interface IPubAdActivities : ICore
    {
        PubAdActivityResponse GetPubAdActivities(int? perPage = null, int? page = null);

        Task<PubAdActivityResponse> GetPubAdActivitiesAsync(int? perPage = null, int? page = null);

        PubAdActivityResponse GetPubAdActivities(DateTime startDate, DateTime endDate,
            int? perPage = null, int? page = null);

        Task<PubAdActivityResponse> GetPubAdActivitiesAsync(DateTime startDate, DateTime endDate,
            int? perPage = null, int? page = null);

    }

    public class PubAdActivities : Core, IPubAdActivities
    {
        /// <summary>
        /// Service Operation name
        /// </summary>
        private const string _pubAdActivity = "PublicationAdActivity";

        /// <summary>
        /// Initalize a new instance of Publication Ads Activities
        /// </summary>
        /// <param name="serviceUrl">Endpoint service URL</param>
        /// <param name="ocp_SubscriptionKey">Subscription key</param>
        public PubAdActivities(string serviceUrl, string ocp_SubscriptionKey)
            : base(serviceUrl, ocp_SubscriptionKey, null)
        {

        }
        /// <summary>
        /// Initalize a new instance of Publication Ads Activities
        /// </summary>
        /// <param name="serviceUrl">Endpoint service URL</param>
        /// <param name="ocp_SubscriptionKey">Subscription key</param>
        /// <param name="proxy">Optional - Web Proxy </param>
        public PubAdActivities(string serviceUrl, string ocp_SubscriptionKey, IWebProxy proxy)
            : base(serviceUrl, ocp_SubscriptionKey, proxy)
        {

        }

        /// <summary>
        /// Get All Publication Activities
        /// </summary>
        /// <param name="startDate">start date of the activity</param>
        /// <param name="endDate">end date of the activity</param>
        /// <returns><seealso cref="PubAdActivityResponse"/> of publication activities</returns>
        public PubAdActivityResponse GetPubAdActivities(int? perPage = null, int? page = null)
        {
            return GetPubAdActivities(DateTime.MinValue, DateTime.Today);
        }

        /// <summary>
        /// Get All Publication Activities
        /// </summary>
        /// <param name="startDate">start date of the activity</param>
        /// <param name="endDate">end date of the activity</param>
        /// <returns><seealso cref="PubAdActivityResponse"/> of publication activities</returns>
        public PubAdActivityResponse GetPubAdActivities(DateTime startDate, DateTime endDate,
            int? perPage = null, int? page = null)
        {
            string resource = _pubAdActivity +
                string.Format("?startDate={0}&endDate={1}",
                    startDate.ToString("yyyy-MM-dd"),
                    endDate.ToString("yyyy-MM-dd"));

            if (perPage.HasValue && page.HasValue)
            {
                return GenericPagedGet<PubAdActivityResponse>(resource, perPage, page);
            }
            else
            {
                return GenericGet<PubAdActivityResponse>(resource);
            }
        }

        /// <summary>
        /// Get Publication Activities Async
        /// </summary>
        /// <param name="perPage">Number of activity per page</param>
        /// <param name="page">Page number</param>
        /// <returns><seealso cref="PubAdActivityResponse"/> of publication activities</returns>
        public Task<PubAdActivityResponse> GetPubAdActivitiesAsync(int? perPage = null, int? page = null)
        {
            if (perPage.HasValue && page.HasValue)
            {
                return GenericPagedGetAsync<PubAdActivityResponse>(_pubAdActivity, perPage, page);
            }
            else
            {
                return GenericGetAsync<PubAdActivityResponse>(_pubAdActivity);
            }
        }

        /// <summary>
        /// Get Publication Activities Async
        /// </summary>
        /// <param name="startDate">start date of the activity</param>
        /// <param name="endDate">end date of the activity</param>
        /// <param name="perPage">Number of activity per page</param>
        /// <param name="page">Page number</param>
        /// <returns><seealso cref="PubAdActivityResponse"/> of publication activities</returns>
        public Task<PubAdActivityResponse> GetPubAdActivitiesAsync(DateTime startDate, DateTime endDate, int? perPage = null, int? page = null)
        {
            string resource = _pubAdActivity +
                string.Format("?startDate={0}&endDate={1}",
                    startDate.ToString("yyyy-MM-dd"),
                    endDate.ToString("yyyy-MM-dd"));

            if (perPage.HasValue && page.HasValue)
            {
                return GenericPagedGetAsync<PubAdActivityResponse>(resource, perPage, page);
            }
            else
            {
                return GenericGetAsync<PubAdActivityResponse>(resource);
            }
        }
    }
}
