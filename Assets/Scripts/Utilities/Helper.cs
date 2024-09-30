namespace Prez.Utilities
{
    public static class Helper
    {
        public static string GetFormattedNumber(int number)
        {
            return number switch
            {
                >= 1000000000 => (number * 0.000000001f).ToString("F2") + "B",
                >= 1000000 => (number * 0.000001f).ToString("F2") + "M",
                >= 1000 => (number * 0.001f).ToString("F2") + "K",
                _ => number.ToString(),
            };
        }
    }
}