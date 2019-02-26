using MediaRadar.API.SDK.Models.PubAdActivities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaRadar.PubAd.WebCore.ViewModel.Activity
{
    public class ActivityMatrix
    {

        public ActivityMatrix(PubAdActivityResponse result)
        {
            //A list of brands that have at least 2 ad pages, and are considered part of the “Toiletries & Cosmetics > Hair Care”
            //product category. This list should also be sortable and paged. Default sort by brand name alphabetically
            FocusedBrands = result
                .Where(r => r.ProductCategory.Equals("Toiletries & Cosmetics > Hair Care", StringComparison.InvariantCultureIgnoreCase) && r.AdPages >= 2)
                .Select(r => r.BrandName)
                .Distinct()
                .OrderBy(b => b);

            // The top five Product Categories by estimated spend. Default sort by number of pages (descending)
            // then product category alphabetically.
            TopProducts = result
                .OrderByDescending(r => r.EstPrintSpend)
                .ThenByDescending(r => r.AdPages)
                .ThenBy(r => r.ProductCategory)
                .Take(5)
                .Select(r => r.ProductCategory);


            //TODO: Fix that grouping
            // The top five Parent Companies by number of pages, then estimated spend during a single month. 
            // Keep in mind that a Parent Company may run ads in multiple issues/months. 
            // Default sort by number of pages (descending), estimated spend(descending), then Parent Company alphabetically.
            TopParentCompanies = result
                .GroupBy(r => r.Month)
                .Select(grp =>
                    new TopParentCompanies()
                    {
                        Month = DateTime.Parse(grp.Key),
                        CompanyNames = grp.OrderByDescending(g => g.AdPages)
                                        .ThenByDescending(g => g.EstPrintSpend)
                                        .ThenBy(g => g.ParentCompany)
                                        .Take(5)
                                        .Select(g => g.ParentCompany)
                                        .ToList()
                    })
                    .OrderByDescending(t => t.Month);
        }

        public IEnumerable<string> FocusedBrands { get; set; }

        public IEnumerable<string> TopProducts { get; set; }

        public IEnumerable<TopParentCompanies> TopParentCompanies { get; set; }

    }
}
