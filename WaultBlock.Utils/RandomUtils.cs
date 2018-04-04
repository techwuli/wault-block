using System;
using System.Linq;

namespace WaultBlock.Utils
{
    public static class RandomUtils
    {
        public static int RandomNumber(int length)
        {
            var minValue = (int)Math.Pow(10, length - 1);
            var maxValue = (int)Math.Pow(10, length);

            var r = new Random();
            var result = r.Next(minValue, maxValue);
            return result;
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
