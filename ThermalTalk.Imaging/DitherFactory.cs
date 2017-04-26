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
