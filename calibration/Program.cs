using System.Collections.Generic;
using System.IO;

namespace calibration
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\tom.swedlund\Documents\GitHub\calibration\";
            Matrix image = ImageUtils.LoadGrayscale(Path.Combine(path, "CalibIm1.gif"));
            Matrix harris;
            List<Vector> corners = ImageUtils.HarrisCornerDetector(image, out harris);

            Matrix shift = new Matrix(harris.Height, harris.Width, Matrix.MaxNeg(harris));
            harris += shift;
            harris /= Matrix.Max(harris);
            ImageUtils.SaveAsImage(harris, Path.Combine(path, "harris.png"));

            Matrix cornerIm = new Matrix(harris.Height, harris.Width);
            foreach (Vector pt in corners)
            {
                cornerIm[(int)pt[0], (int)pt[1]] = 1;
            }
            ImageUtils.SaveAsImage(cornerIm, Path.Combine(path, "corner.png"));


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
