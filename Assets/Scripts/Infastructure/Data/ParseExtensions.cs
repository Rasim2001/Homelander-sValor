using System.Globalization;

namespace Infastructure.Data
{
    public static class ParseExtensions
    {
        public static int ToInt(this string value, int defaultValue = 0) =>
            int.TryParse(value, out int result) ? result : defaultValue;

        public static float ToFloat(this string value, float defaultValue = 0.0f)
        {
            return float.TryParse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out float result)
                ? result
                : defaultValue;
        }

        
    }
}