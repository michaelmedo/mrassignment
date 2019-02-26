using Microsoft.AspNetCore.Mvc;
using System;

namespace MediaRadar.PubAd.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

     

    }
}
