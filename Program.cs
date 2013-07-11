using System.Collections.Generic;
namespace calibration
{
    class Program
    {
        static void Main(string[] args)
        {
            Matrix image = ImageUtils.LoadGrayscale(@"C:\Users\tom\Documents\Visual Studio 2012\Projects\calibration\calibration\calib.gif");
            List<Vector> corners = ImageUtils.HarrisCornerDetector(image);



            ////image = ImageUtils.ConvertToBinary(image, 0.8f);
            ////ImageUtils.SaveAsImage(image, @"C:\Users\tom\Documents\Visual Studio 2012\Projects\calibration\calibration\binary.bmp");

            ////Matrix sobel = ImageUtils.ConvolveSobelX(image);
            ////sobel += ImageUtils.ConvolveSobelY(image);

            //Matrix sobel = ImageUtils.HarrisCornerDetector(image);
            //Matrix shift = new Matrix(sobel.Height, sobel.Width, Matrix.MaxNeg(sobel));
            //sobel += shift;
            //sobel /= Matrix.Max(sobel);

            //ImageUtils.SaveAsImage(sobel, @"C:\Users\tom\Documents\Visual Studio 2012\Projects\calibration\calibration\result.bmp");
        }
    }
}
