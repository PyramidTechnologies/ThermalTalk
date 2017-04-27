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
    public static class DitherFactory
    {
        // We do not use jagged arrays because I feel that syntax of multi-dimensional
        // arrays is easier to work with
       [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
       public static IDitherable GetDitherer(Algorithms algorithm, byte threshold = 128)
        {
           switch(algorithm)
           {
               case Algorithms.Atkinson:
                   // 0,0,1,1,1,1,1,0,0,1,0,0
                   return new Dither(new byte[,] {
                       {0,0,1,1},
                       {1,1,1,0},
                       {0,1,0,0}
                   }, 3, threshold, true);

               case Algorithms.Burkes:
                   // 0,0,0,8,4,2,4,8,4,2
                   return new Dither(new byte[,] {
                       {0,0,0,8,4},
                       {2,4,8,4,2},
                   }, 5, threshold, true);

               case Algorithms.FloydSteinberg:
                   // 0,0,7,3,5,1
                   return new Dither(new byte[,] {
                       {0,0,7},
                       {3,5,1},
                   }, 4, threshold, true);

               case Algorithms.FloydSteinbergFalse:
                   // 0,3,3,2
                   return new Dither(new byte[,] {
                       {0,3},
                       {3,2},
                   }, 3, threshold, true);

               case Algorithms.JarvisJudiceNinke:
                   // 0,0,0,7,5,3,5,7,5,3,1,3,5,3,1
                   return new Dither(new byte[,] {
                       {0,0,0,7,5},
                       {3,5,7,5,3},
                       {1,3,5,3,1}
                   }, 48, threshold);

               case Algorithms.Sierra:
                   // 0,0,0,5,3,2,4,5,4,2,0,2,3,2,0
                   return new Dither(new byte[,] {
                       {0,0,0,5,3},
                       {2,4,5,4,2},
                       {0,2,3,2,0},
                   }, 5, threshold, true);

               case Algorithms.Sierra2:
                   // 0,0,0,4,3,1,2,3,2,1
                   return new Dither(new byte[,] {
                       {0,0,0,4,3},
                       {1,2,3,2,1},
                   }, 4, threshold, true);

               case Algorithms.SierraLite:
                   // 0,0,2,1,1,0
                   return new Dither(new byte[,] {
                       {0,0,2},
                       {1,1,0},
                   }, 2, threshold, true);

               case Algorithms.Stucki:
                   // 0,0,0,8,4,2,4,8,4,2,1,2,4,2,1
                   return new Dither(new byte[,] {
                       {0,0,0,8,4},
                       {2,4,8,4,2},
                       {1,2,4,2,1},
                   }, 42, threshold);

               default:
                   // We need to at least make it 1bpp bitmap otherwise phoenix will have garbage.
                   return new OneBPP(threshold);
           }
        }
    }
}
