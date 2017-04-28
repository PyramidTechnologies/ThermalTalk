using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Drawing;
using System.IO;

namespace ThermalTalk.Imaging.Test
{
    [TestFixture]
    public class DitherFactoryTests
    {
        private const string baseDir = "test_data";

        [OneTimeSetUp]
        public void Setup()
        {
            var dir = Path.GetDirectoryName(typeof(DitherFactoryTests).Assembly.Location);
            Directory.SetCurrentDirectory(dir);

            if(Directory.Exists(baseDir))
            {
                Directory.Delete(baseDir, true);
            }

            Directory.CreateDirectory(baseDir);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (Directory.Exists(baseDir))
            {
                Directory.Delete(baseDir, true);
            }
        }

        /// <summary>
        /// Generates various perfect-gray input dithers and compares to known
        /// good dither generators, e.g. photoshop
        /// </summary>
        [Test()]
        public void GetDithererAtkinsonTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;


            expected = Properties.Resources.gray_atkinson;
            dithered = DitherFactory.GetDitherer(Algorithms.Atkinson).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_atkinson.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_Atkinson.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));
        }

        [Test()]
        public void GetDithererBurkesTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_burkes;
            dithered = DitherFactory.GetDitherer(Algorithms.Burkes).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_burkes.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_Burkes.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }

        [Test()]
        public void GetDithererFloydSteinbergTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_floydsteinbergs;
            dithered = DitherFactory.GetDitherer(Algorithms.FloydSteinberg).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_floydsteinbergs.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_FloydSteinbergs.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));


        }

        [Test()]
        public void GetDithererFloydSteinbergFalseTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            // TODO - Find a good false floyd stein generator
            {
                //expected = Properties.Resources.gray_floydsteinbergs_false;
                dithered = DitherFactory.GetDitherer(Algorithms.FloydSteinbergFalse).GenerateDithered(input);
                if (debugSave)
                {
                    //expected.Save(Path.Combine(baseDir, "gray_floydsteinbergs_false.bmp");
                    dithered.Save(Path.Combine(baseDir, "actual_FloydSteinbergFalse.bmp"));
                }
                //Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));
            }


        }

        [Test()]
        public void GetDithererJarvisJudiceNinkeTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_jjn;
            dithered = DitherFactory.GetDitherer(Algorithms.JarvisJudiceNinke).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_jjn.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_JarvisJudiceNinke.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }

        [Test()]
        public void GetDithererNoneTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.white_bitmap;
            dithered = DitherFactory.GetDitherer(Algorithms.None).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_bitmap.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_None.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }

        [Test()]
        public void GetDithererSierraTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_sierra;
            dithered = DitherFactory.GetDitherer(Algorithms.Sierra).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_sierra.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_Sierra.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }

        [Test()]
        public void GetDithererSierra2Test()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_sierra2;
            dithered = DitherFactory.GetDitherer(Algorithms.Sierra2).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_sierra2.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_Sierra2.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }

        [Test()]
        public void GetDithererSierraLiteTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_sierralite;
            dithered = DitherFactory.GetDitherer(Algorithms.SierraLite).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_sierralite.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_SierraLite.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }

        [Test()]
        public void GetDithererStuckiTest()
        {

            bool debugSave = true;

            // Input are expected are provided as resources, dithered is what
            // we are testing
            Bitmap input, dithered, expected;
            input = Properties.Resources.gray_bitmap;
            expected = Properties.Resources.gray_stucki;
            dithered = DitherFactory.GetDitherer(Algorithms.Stucki).GenerateDithered(input);
            if (debugSave)
            {
                expected.Save(Path.Combine(baseDir, "gray_stucki.bmp"));
                dithered.Save(Path.Combine(baseDir, "actual_Stucki.bmp"));
            }
            Assert.IsTrue(ImageTestHelpers.CompareMemCmp(expected, dithered));

        }


        [Test()]
        public void BadDitherCtorTest()
        {
            // Cannot have a null algorith matrix
            ActualValueDelegate<object> testDelegate1 = () =>
                new Dither(null, 0, 1);
            Assert.That(testDelegate1, Throws.TypeOf<ArgumentNullException>());

            // Cannot allow a zero divisor
            ActualValueDelegate<object> testDelegate2 = () =>
                new Dither(new byte[,] { }, 0, 0);
            Assert.That(testDelegate2, Throws.TypeOf<ArgumentException>());
        }
    }
}
