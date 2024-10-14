using System.Globalization;
using Core;

namespace Utilities
{
    public static class Helper
    {
        /// <summary>
        /// Formats a double number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetNumberAsString(double number)
        {
            var numberString = string.Empty;

            if (number < 0)
            {
                numberString = "-";
                number *= -1;
            }

            if (GameManager.Data.UseENotation)
                return numberString + GetNumberAsENotationString(number);

            if (number >= 1000000000000000000000000000d)
            {
                numberString += number switch
                {
                    >= 10000000000000000000000000000000000d => (number * 0.000000000000000000000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "d",
                    >= 1000000000000000000000000000000d => (number * 0.000000000000000000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "n",
                    _ => (number * 0.000000000000000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "o",
                };
            }
            else if (number >= 1000000000000000d)
            {
                numberString += number switch
                {
                    >= 1000000000000000000000000d => (number * 0.000000000000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "S",
                    >= 1000000000000000000000d => (number * 0.000000000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "s",
                    >= 1000000000000000000d => (number * 0.000000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "Q",
                    _ => (number * 0.000000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "q",
                };
            }
            else
            {
                numberString += number switch
                {
                    >= 1000000000000d => (number * 0.000000000001d).ToString("F2", CultureInfo.InvariantCulture) + "T",
                    >= 1000000000d => (number * 0.000000001d).ToString("F2", CultureInfo.InvariantCulture) + "B",
                    >= 1000000d => (number * 0.000001d).ToString("F2", CultureInfo.InvariantCulture) + "M",
                    >= 1000d => (number * 0.001d).ToString("F2", CultureInfo.InvariantCulture) + "K",
                    _ => number.ToString("F2", CultureInfo.InvariantCulture),
                };
            }

            return numberString;
        }

        /// <summary>
        /// Formats a double number as e notation.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string GetNumberAsENotationString(double number)
        {
            if (number < 1000)
                return number.ToString("0.00");
            
            var numberString = number.ToString("0");
            var length = numberString.Length;
            return (number / double.Parse("1".PadRight(length, '0'))).ToString("0.00") + $"e{length - 1:00}";
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