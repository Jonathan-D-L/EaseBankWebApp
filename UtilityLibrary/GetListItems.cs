using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UtilityLibrary
{
    public static class GetListItems
    {
        public static List<SelectListItem> GetGenders()
        {
            var genders = new List<string> { "non-binary", "male", "female" };
            return genders.Select(x => new SelectListItem
            {
                Value = x,
                Text = x
            }).ToList();
        }
        public static List<SelectListItem> GetCounties()
        {
            var counties = new List<string> { "Sweden", "Denmark", "Norway", "Finland", };
            return counties.Select(x => new SelectListItem
            {
                Value = x,
                Text = x
            }).ToList();
        }
        public static List<SelectListItem> GetFrequency()
        {
            var counties = new List<string> { "Monthly", "Weekly", "After Transaction", };
            return counties.Select(x => new SelectListItem
            {
                Value = x,
                Text = x
            }).ToList();
        }
    }
}
