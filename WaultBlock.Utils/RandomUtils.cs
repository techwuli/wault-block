using System;

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
    }
}
