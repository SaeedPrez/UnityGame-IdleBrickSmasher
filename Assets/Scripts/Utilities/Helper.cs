using System.Globalization;

namespace Prez.Utilities
{
    public static class Helper
    {
        /// <summary>
        /// Formats a double number.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="decimalPoints"></param>
        /// <returns></returns>
        public static string GetNumberAsString(double number)
        {
            return number switch
            {
                >= 1000000000000 => (number * 0.000000001d).ToString("F2", CultureInfo.InvariantCulture) + "T",
                >= 1000000000 => (number * 0.000000001d).ToString("F2", CultureInfo.InvariantCulture) + "B",
                >= 1000000 => (number * 0.000001d).ToString("F2", CultureInfo.InvariantCulture) + "M",
                >= 1000 => (number * 0.001d).ToString("F2", CultureInfo.InvariantCulture) + "K",
                _ => number.ToString("F2", CultureInfo.InvariantCulture),
            };
        }
        
        /// <summary>
        /// Calculates cost.
        /// </summary>
        /// <param name="baseCost"></param>
        /// <param name="growth"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static double CalculateLevelCost(double baseCost, double growth, double level)
        {
            var result = 0d;
            
            for (var i = 1; i <= level; i++)
                result = (result + baseCost) * growth;

            return result;
        }
    }
}