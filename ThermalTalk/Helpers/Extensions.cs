namespace ThermalTalk
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class Extensions
    {

        /// <summary>
        /// Split the given array into x number of smaller arrays, each of length len
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayIn"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static T[][] Split<T>(this T[] arrayIn, int len)
        {
            bool even = arrayIn.Length % len == 0;
            int totalLength = arrayIn.Length / len;
            if (!even)
                totalLength++;

            T[][] newArray = new T[totalLength][];
            for (int i = 0; i < totalLength; ++i)
            {
                int allocLength = len;
                if (!even && i == totalLength - 1)
                    allocLength = arrayIn.Length % len;

                newArray[i] = new T[allocLength];
                Array.Copy(arrayIn, i * len, newArray[i], 0, allocLength);
            }

            return newArray;
        }

        /// <summary>
        /// Concatentates all arrays into one
        /// </summary>
        /// <param name="args">1 or more byte arrays</param>
        /// <returns>byte[]</returns>
        public static byte[] Concat(params byte[][] args)
        {
            using (var buffer = new MemoryStream())
            {
                foreach (var ba in args)
                {
                    buffer.Write(ba, 0, ba.Length);
                }

                var bytes = new byte[buffer.Length];
                buffer.Position = 0;
                buffer.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }

        /// <summary>
        /// Returns all flags that are set in this value in ascending order.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>Enumerates get flags in this collection.</summary>
        ///
        /// <param name="value">The value.
        /// </param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process get flags in this collection.</returns>
        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<T>().ToArray());
        }

        /// <summary>Enumerates get flags in this collection.</summary>
        ///
        /// <param name="value"> The value.
        /// </param>
        /// <param name="values">The values.
        /// </param>
        ///
        /// <returns>An enumerator that allows foreach to be used to process get flags in this collection.</returns>
        private static IEnumerable<T> GetFlags<T>(T value, T[] values) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type must be an enum.");
            }
            ulong bits = Convert.ToUInt64(value);
            var results = new List<T>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<T>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<T>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<T>();
        }
    }
}
