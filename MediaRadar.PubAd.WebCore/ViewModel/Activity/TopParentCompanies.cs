using System;
using System.Collections.Generic;

namespace MediaRadar.PubAd.WebCore.ViewModel.Activity
{
    public class TopParentCompanies
    {
        public DateTime Month { get; set; }

        public IEnumerable<string> CompanyNames { get; set; }
    }
}