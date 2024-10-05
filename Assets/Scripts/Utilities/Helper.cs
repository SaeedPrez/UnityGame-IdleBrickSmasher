using System.Globalization;

namespace Prez.Utilities
{
    public static class Helper
    {
        /// <summary>
        /// Get number as decimal.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static decimal GetNumberAsDecimal(long number)
        {
            return number / 100m;
        }
        
        /// <summary>
        /// Formats a float number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetNumberAsString(long number)
        {
            var decimalNumber = GetNumberAsDecimal(number);
            
            return decimalNumber switch
            {
                >= 1000000000000 => (decimalNumber * 0.000000001m).ToString("F2") + "T",
                >= 1000000000 => (decimalNumber * 0.000000001m).ToString("F2") + "B",
                >= 1000000 => (decimalNumber * 0.000001m).ToString("F2") + "M",
                >= 1000 => (decimalNumber * 0.001m).ToString("F2") + "K",
                _ => decimalNumber.ToString(CultureInfo.InvariantCulture),
            };
        }
    }
}