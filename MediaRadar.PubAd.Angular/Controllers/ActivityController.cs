using MediaRadar.API.SDK;
using MediaRadar.API.SDK.Models.PubAdActivities;
using MediaRadar.PubAd.WebCore.ViewModel.Activity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MediaRadar.PubAd.Angular.Controllers
{
    [Route("api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IPublicationAdsAPI _api = null;
        private readonly DateTime startDate = new DateTime(2011, 1, 1);
        private readonly DateTime endDate = new DateTime(2011, 4, 1);

        public ActivityController(IPublicationAdsAPI api)
        {
            _api = api;
        }

        [HttpGet("[action]")]
        public PubAdActivityResponse List()
        {
            return _api.PubAdActivities.GetPubAdActivities(startDate, endDate);
        }
        [HttpGet("[action]")]
        public ActivityMatrix Dashboard()
        {
            var result = _api.PubAdActivities.GetPubAdActivities(startDate, endDate);
            return new ActivityMatrix(result);
        }
    }
}
