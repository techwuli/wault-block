using System;

namespace WaultBlock.Utils
{
    public static class ParamsGuard
    {
        public static void NotNull(object param, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void NotNullOrWhiteSpace(string param, string paramName)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException("A value must be provided.", paramName);
            }
        }

        public static void NotNullOrEmpty(string param, string paramName)
        {
            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentException("A value must be provided.", paramName);
            }
        }
    }
}
