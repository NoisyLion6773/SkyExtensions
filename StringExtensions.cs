using System.Text.RegularExpressions;

namespace SkyExtensions;

public static class StringExtensions
{
    public static void RemoveWhitespaces(this string var)
    {
        var = Regex.Replace(var, @"\s+", "");
    }

    public static int ToInt(this string theValue, int? DefaultValue = 0)
    {
        if (theValue.IsNumeric())
        {
            return int.Parse(theValue);
        }
        if (DefaultValue == null)
            return 0;
        else
            return (int)DefaultValue;
    }

    public static bool IsNumeric(this string theValue)
    {
        long retNum;
        return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
    }

}
