using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibrary
{
    public static class CountryCodeMapper
    {
        public static string GetFlag(string countryCode)
        {
            var flag = string.Empty;
            flag = countryCode switch
            {
                "SE" => "/images/se.png",
                "DK" => "/images/dk.png",
                "NO" => "/images/no.png",
                "FI" => "/images/fi.png",
                _ => flag
            };
            return flag;
        }
        public static string GetCurrency(string countryCode)
        {
            var currency = "SEK";
            //no system for exchange rates hence this is commented out
            //currency = countryCode switch
            //{
            //    "SE" => "SEK",
            //    "DK" => "DKK",
            //    "NO" => "NOK",
            //    "FI" => "EUR",
            //    _ => currency
            //};
            //no system for exchange rates hence this is commented out
            return currency;
        }
        public static string GetCountry(string countryCode)
        {
            var country = string.Empty;
            country = countryCode switch
            {
                "SE" => "Sweden",
                "DK" => "Denmark",
                "NO" => "Norway",
                "FI" => "Finland",
                _ => country
            };
            return country;
        }
    }
}
