using System.Collections.Generic;
using System.IO;
using Tom.Math;

namespace calibration
{
    class Program
    {
        static void Main(string[] args)
        {
            //string path = @"C:\Users\tom.swedlund\Documents\GitHub\calibration\";
            string path = @"c:\Users\tom\Documents\GitHub\calibration\";
            Matrix image = ImageUtils.LoadGrayscale(Path.Combine(path, "CalibIm1.gif"));
            Matrix harris;
            List<Vector> corners = ImageUtils.HarrisCornerDetector(image, out harris);

            foreach (Vector corner in corners)
            {
                image[(int)corner[0], (int)corner[1]] = 1;
            }
            ImageUtils.SaveAsImage(image, Path.Combine(path, "overlay.png"));

            Matrix shift = new Matrix(harris.Height, harris.Width, MatrixUtils.MaxNeg(harris));
            harris += shift;
            harris /= MatrixUtils.Max(harris);
            ImageUtils.SaveAsImage(harris, Path.Combine(path, "harris.png"));

            Matrix cornerIm = new Matrix(harris.Height, harris.Width);
            foreach (Vector pt in corners)
            {
                cornerIm[(int)pt[0], (int)pt[1]] = 1;
            }
            ImageUtils.SaveAsImage(cornerIm, Path.Combine(path, "corner.png"));
        }
    }
}
