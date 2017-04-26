
namespace ThermalTalk.Imaging
{
    using System;
    using System.Collections.Generic;

    static class Extensions
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
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int RoundUp(this int i, int N)
        {
            return (int)RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static long RoundUp(this long i, int N)
        {
            return RoundUp(i, (uint)N);
        }

        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static uint RoundUp(this uint i, int N)
        {
            return (uint)RoundUp(i, (uint)N);
        }



        /// <summary>
        /// Rounds this integer to the nearest positive multiple of N
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static long RoundUp(long i, uint N)
        {
            if (N == 0)
            {
                return 0;
            }

            if (i == 0)
            {
                return N;
            }

            return (long)(Math.Ceiling(Math.Abs(i) / (double)N) * N);
        }
    }
}
