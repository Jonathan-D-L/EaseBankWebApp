using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibrary
{
    public static class Format
    {
        public static string FormatTitleCase(string input)
        {
            return string.Join(" ", input.Split()
                .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower()));
        }

        public static string FormatIntToDate(string value)
        {
            int.TryParse(value, out int val);
            return val < 10
                ? string.Join("", "0", value)
                : value;
        }
    }
}
