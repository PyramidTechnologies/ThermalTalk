#region Copyright & License
/*
MIT License

Copyright (c) 2017 Pyramid Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion
namespace ThermalTalk.Imaging
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    static class Extensions
    {
        /// <summary>
        /// Split the given array into x number of smaller arrays, each of length len
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayIn"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        internal static T[][] Split<T>(this T[] arrayIn, int len)
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
        /// Returns a new array sliced from this array similar
        /// to Python/Go notation array[0:4]
        /// </summary>
        /// <param name="start">Start index</param>
        /// <param name="len">Size of slice</param>
        /// <returns>Slice of array</returns>
        internal static T[] Slice<T>(this T[] arr, int start, int len)
        {
            var result = new T[len];
            Array.Copy(arr, start, result, 0, len);

            return result;
        }

        /// <summary>
        /// Fills array with val until arrary length is equal to newSize
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="arr">Arrray to fill</param>
        /// <param name="newSize">New total count of elements</param>
        /// <param name="val">Valur to pad with</param>
        /// <returns>New array with newSize count of elements</returns>
        internal static T[] Pad<T>(this T[] arr, int newSize, T val)
        {
            if (arr.Length < newSize)
            {
                var filler = Repeated<T>(val, newSize - arr.Length).ToArray();
                var result = new T[newSize];
                Array.Copy(arr, result, arr.Length);
                Array.Copy(filler, 0, result, arr.Length-1, filler.Length);
            }
               
            return arr;
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
        /// Returns a list of type T with value repeat count times
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static List<T> Repeated<T>(T value, int count)
        {
            List<T> ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
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
