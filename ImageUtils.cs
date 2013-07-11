using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace calibration
{
    public static class ImageUtils
    {
        public static Matrix LoadGrayscale(string path)
        {
            Bitmap image = new Bitmap(path);
            Matrix mat = new Matrix(image.Height, image.Width);
            if (image.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                for (int row = 0; row < image.Height; ++row)
                {
                    for (int col = 0; col < image.Width; ++col)
                    {
                        mat[row, col] = image.GetPixel(col, row).R / byte.MaxValue;
                    }
                }
            }
            else
            {
                for (int row = 0; row < image.Height; ++row)
                {
                    for (int col = 0; col < image.Width; ++col)
                    {
                        mat[row, col] = ConvertToGrayscale(image.GetPixel(col, row));
                    }
                }
            }
            return mat;
        }

        public static float ConvertToGrayscale(Color color)
        {
            return (0.299f * color.B + 0.587f * color.G + 0.114f * color.B) / byte.MaxValue;
        }

        public static void SaveAsImage(Matrix matrix, string path)
        {
            Bitmap image = new Bitmap(matrix.Width, matrix.Height, PixelFormat.Format24bppRgb);
            for (int row = 0; row < matrix.Height; ++row)
            {
                for (int col = 0; col < matrix.Width; ++col)
                {
                    short color = (short)(Math.Min(matrix[row, col], 1.0f) * byte.MaxValue + 0.5f);
                    //byte color = Math.Max(Math.Min((byte)(matrix[row, col] + 0.5f), byte.MaxValue), byte.MinValue);
                    image.SetPixel(col, row, Color.FromArgb(color, color, color));
                }
            }
            image.Save(path);
        }

        public static Matrix ConvolveSobelX(Matrix matrix)
        {
            Matrix sobelKernel = new Matrix(3, 3);
            sobelKernel[0, 0] = -1; sobelKernel[0, 2] = 1;
            sobelKernel[1, 0] = -2; sobelKernel[1, 2] = 2;
            sobelKernel[2, 0] = -1; sobelKernel[2, 2] = 1;
            return Matrix.Convolve(matrix, sobelKernel);
        }

        public static Matrix ConvolveSobelY(Matrix matrix)
        {
            Matrix sobelKernel = new Matrix(3, 3);
            sobelKernel[0, 0] = -1; sobelKernel[0, 1] = -2; sobelKernel[0, 2] = -1;
            sobelKernel[2, 0] = 1; sobelKernel[2, 1] = 2; sobelKernel[2, 2] = 1;
            return Matrix.Convolve(matrix, sobelKernel);
        }

        public static Matrix ConvertToBinary(Matrix matrix, float threshold)
        {
            Matrix result = new Matrix(matrix.Height, matrix.Width);
            for (int row = 0; row < matrix.Height; ++row)
            {
                for (int col = 0; col < matrix.Width; ++col)
                {
                    result[row, col] = (matrix[row, col] < threshold) ? 0 : 1;
                }
            }
            return result;
        }

        public static List<Vector> HarrisCornerDetector(Matrix image)
        {
            //Matrix dx = ConvolveSobelX(image);
            //Matrix dy = ConvolveSobelY(image);
            //Matrix results = new Matrix(dx.Height, dx.Width);
            //for (int row = 0; row < results.Height; ++row)
            //{
            //    for (int col = 0; col < results.Width; ++col)
            //    {
            //        float ixix = dx[row, col] * dx[row, col];
            //        float ixiy = dx[row, col] * dy[row, col];
            //        float iyiy = dy[row, col] * dy[row, col];

            //        float det = ixix * iyiy + ixiy * ixiy;
            //        float trace = ixiy + ixiy;
            //        float metric = det - 0.04f * trace * trace;
            //        results[row, col] = metric;
            //    }
            //}
            //return results;

            Matrix dx = ConvolveSobelX(image);
            Matrix dy = ConvolveSobelY(image);
            List<Vector> results = new List<Vector>();
            int windowSize = 3;
            for (int row = 0; row < dx.Height; row += windowSize)
            {
                for (int col = 0; col < dx.Width; col += windowSize)
                {
                    Vector maxPos = null;
                    Matrix derivMat = GenerateDerivMatrix(image, dx, dy, col, row, windowSize, out maxPos);
                    float metric = HarrisMetric(derivMat);
                    if (metric > 100000)
                    {
                        results.Add(maxPos);
                    }
                }
            }
            return results;
        }

        private static Matrix GenerateDerivMatrix(Matrix image, Matrix dx, Matrix dy, int x, int y, int windowSize, out Vector maxPos)
        {
            Matrix result = new Matrix(2, 2);
            maxPos = new Vector(2);
            float max = float.MinValue;
            for (int row = y; row < image.Height; ++row)
            {
                for (int col = x; col < image.Width; ++col)
                {
                    float corner = dx[y, x] * dy[y, x];
                    Matrix current = new Matrix(2, 2);
                    current[0, 0] += (dx[y, x] * dx[y, x]);
                    current[0, 1] += corner;
                    current[1, 0] += corner;
                    current[1, 1] += (dy[y, x] * dy[y, x]);
                    result += current;
                    float metric = HarrisMetric(current);
                    if (metric > max)
                    {
                        maxPos = new Vector(2);
                        maxPos[0] = col;
                        maxPos[1] = row;
                        max = metric;
                    }
                }
            }
            return result;
        }

        private static float HarrisMetric(Matrix derivMatrix)
        {
            float det = derivMatrix[0, 0] * derivMatrix[1, 1] + derivMatrix[1, 0] * derivMatrix[0, 1];
            float trace = derivMatrix[0, 0] + derivMatrix[1, 1];
            return (det - 0.04f * trace * trace);
        }
    }
}
