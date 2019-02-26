using MediaRadar.API.SDK;
using MediaRadar.PubAd.WebCore.ViewModel.Activity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MediaRadar.PubAd.Web.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IPublicationAdsAPI _api = null;
        private readonly DateTime startDate = new DateTime(2011, 1, 1);
        private readonly DateTime endDate = new DateTime(2011, 4, 1);

        public ActivityController(IPublicationAdsAPI api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var result =
                await _api.PubAdActivities.GetPubAdActivitiesAsync(startDate, endDate);

            return View(new ActivityMatrix(result));
        }

        public async Task<IActionResult> Advertisers()
        {
            var model =
                await _api.PubAdActivities.GetPubAdActivitiesAsync(startDate, endDate);
            return View(model);
        }
    }
}