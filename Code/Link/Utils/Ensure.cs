using System;

namespace WALLE.Link.Utils
{
    public static class Ensure
    {
        public static void IsNotNull<TValue>(TValue value, string nameOfValue) where TValue : class
        {
            if (value == null)
                throw new ArgumentNullException(nameOfValue);
        }

        public static void IsMoreThan<TValue>(TValue value, TValue comparedWith, string nameOfValue) where TValue : IComparable<TValue>
        {
            if (value.CompareTo(comparedWith) <= 0)
                throw new ArgumentOutOfRangeException(nameOfValue);
        }
    }
}
