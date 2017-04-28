# Imaging Module

This module provides a collection of image manipulations algorithms and functions. The primary features are:

* Image dithering
* Quick Bitmap to buffer and buffer to Bitmap conversions
* Lossless Resize
* Generate packet-ready data streams for serial transmission


## Examples

All example require the following using statement

    using ThermalTalk.Imaging;


### Dithering
Dithering is the process of converting a high-resolution image into a low-res image
without loosing too much fidelity. There are a number of algorithms available and 
they each provide difference balances between quality and performance. My favorite 
is JarvisJudiceNinke because it looks good and runs quickly.

    var bitmap = @"C:\path\to\my\image.jpg" // Accepts all common image file types
    var parser = DitherFactory.GetDitherer(Algorithms.JarvisJudiceNinke); // Use default gray threshold
    var dithered = parser.GenerateDithered(bitmap);



### Phoenix Logo Generation
Here is a sample specific to Phoenix. Given an image, generate a byte[] than is ready to be
packaged in a packet protocol and transmitted to a physical device.

    var bitmap = @"C:\some_image.png";
    var logoBuff = bitmap.ToLogoBuffer();


### Converting a buffer into a bitmap
Sometimes we want to manipulate a bitmap in memory or visual a buffer as a bitmap.

    var my buff = new byte[] {....} // Your data
    var width = 200;                // Bitmap width in bytes
    var height = 200;               // Bitmap height in bytes
    var bitmap = buff.AsBitmap(width, height);
