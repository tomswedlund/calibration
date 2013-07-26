using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Tom.Math;

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
            return MatrixUtils.Convolve(matrix, sobelKernel);
        }

        public static Matrix ConvolveSobelY(Matrix matrix)
        {
            Matrix sobelKernel = new Matrix(3, 3);
            sobelKernel[0, 0] = -1; sobelKernel[0, 1] = -2; sobelKernel[0, 2] = -1;
            sobelKernel[2, 0] = 1; sobelKernel[2, 1] = 2; sobelKernel[2, 2] = 1;
            return MatrixUtils.Convolve(matrix, sobelKernel);
        }

        public static List<Vector> HarrisCornerDetector(Matrix image, out Matrix harrisImage)
        {
            List<Vector> corners = new List<Vector>();
            Matrix dx = ConvolveSobelX(image);
            Matrix dy = ConvolveSobelY(image);
            Matrix ixix = MatrixUtils.MultElements(dx, dx);
            Matrix ixiy = MatrixUtils.MultElements(dx, dy);
            Matrix iyiy = MatrixUtils.MultElements(dy, dy);
            int windowSize = 3;
            harrisImage = new Matrix(ixix.Height, ixix.Width);
            for (int row = 0; row < dx.Height; ++row)
            {
                for (int col = 0; col < dx.Width; ++col)
                {
                    Matrix derivMat = GenerateDerivMatrix(ixix, ixiy, iyiy, col, row, windowSize);
                    float harrisResponse = KanadeResponse(derivMat[0, 0], derivMat[0, 1], derivMat[1, 1]);
                    harrisImage[row, col] = harrisResponse;
                }
            }

            NonMaxSupression(harrisImage, corners);
            return corners;
        }
        
        private static Matrix GenerateDerivMatrix(Matrix ixix, Matrix ixiy, Matrix iyiy, int x, int y, int windowSize)
        {
            Matrix result = new Matrix(2, 2);
            int halfWindowSize = windowSize / 2;
            int startRow = Math.Max(0, y - halfWindowSize);
            int startCol = Math.Max(0, x - halfWindowSize);
            int endRow = Math.Min(ixix.Height, y + halfWindowSize);
            int endCol = Math.Min(ixix.Width, x + halfWindowSize);
            for (int row = startRow; row < endRow; ++row)
            {
                for (int col = startCol; col < endCol; ++col)
                {
                    Matrix current = new Matrix(2, 2);
                    current[0, 0] += ixix[row, col];
                    current[0, 1] += ixiy[row, col];
                    current[1, 0] += ixiy[row, col];
                    current[1, 1] += iyiy[row, col];
                    result += current;
                }
            }
            return result;
        }

        private static void NonMaxSupression(Matrix harris, List<Vector> corners)
        {
            List<Tuple<float, int, int>> cornerVals = new List<Tuple<float,int,int>>();
            for (int row = 0; row < harris.Height; ++row)
            {
                for (int col = 0; col < harris.Width; ++col)
                {
                    //if (harris[row, col] > 40)
                    if (harris[row, col] > 2.5)
                    {
                        Tuple<float, int, int> tuple = new Tuple<float,int,int>(harris[row, col], row, col);
                        cornerVals.Add(tuple);
                    }
                }
            }
            cornerVals.Sort((a, b) => b.Item1.CompareTo(a.Item1));

            SparseMatrix sparse = new SparseMatrix(harris.Height, harris.Width);
            foreach (Tuple<float, int, int> tuple in cornerVals)
            {
                if (!NeighborhoodHasValue(sparse, tuple.Item3, tuple.Item2, 3))
                {
                    sparse[tuple.Item2, tuple.Item3] = tuple.Item1;
                    Vector v = new Vector(2); v[0] = tuple.Item2; v[1] = tuple.Item3;
                    corners.Add(v);
                }
            }
        }

        private static bool NeighborhoodHasValue(SparseMatrix corners, int x, int y, int windowSize)
        {
            int halfWindowSize = windowSize / 2;
            int startRow = Math.Max(0, y - halfWindowSize);
            int startCol = Math.Max(0, x - halfWindowSize);
            int endRow = Math.Min(corners.Height, y + halfWindowSize);
            int endCol = Math.Min(corners.Width, x + halfWindowSize);
            for (int row = startRow; row <= endRow; ++row)
            {
                for (int col = startCol; col <= endCol; ++col)
                {
                    if (corners[row, col] != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Vector NonMaxSupressionWindow(Matrix harris, int x, int y, int windowSize)
        {
            int halfWindowSize = windowSize / 2;
            int startRow = Math.Max(0, y - halfWindowSize);
            int startCol = Math.Max(0, x - halfWindowSize);
            int endRow = Math.Min(harris.Height, y + halfWindowSize);
            int endCol = Math.Min(harris.Width, x + halfWindowSize);
            float max = float.MinValue;
            Vector maxPoint = new Vector(2);
            for (int row = startRow; row < endRow; ++row)
            {
                for (int col = startCol; col < endCol; ++col)
                {
                    if (harris[row, col] > max)
                    {
                        max = harris[row, col];
                        maxPoint[0] = row;
                        maxPoint[1] = col;
                    }
                }
            }
            return maxPoint;
        }

        private static float HarrisResponse(float ixix, float ixiy, float iyiy)
        {
            float det = ixix * iyiy - ixiy * ixiy;
            float trace = ixiy + ixiy;
            return (det - 0.04f * trace * trace);
        }

        private static float KanadeResponse(float ixix, float ixiy, float iyiy)
        {
            float B = -ixix - iyiy;
            float C = -ixiy * ixiy + ixix * iyiy;
            float plus, minus;
            if (MathUtils.SolveQuadratic(1, B, C, out plus, out minus))
            {
                return Math.Min(plus, minus);
            }
            else
            {
                return 0;
            }
        }
    }
}
