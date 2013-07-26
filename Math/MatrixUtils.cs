using System;

namespace Tom.Math
{
    public static class MatrixUtils
    {
         public static Matrix Convolve(Matrix matrix, Matrix kernel)
        {
            int newWidth = matrix.Width - kernel.Width + 1;
            int newHeight = matrix.Height - kernel.Height + 1;
            if ((newWidth <= 0) || (newHeight <= 0))
            {
                throw new ArgumentException("Kernel too large.");
            }

            Matrix result = new Matrix(newHeight, newWidth);
            for (int row = 0; row < result.Height; ++row)
            {
                for (int col = 0; col < result.Width; ++col)
                {
                    float value = 0;
                    for (int kernelRow = 0; kernelRow < kernel.Height; ++kernelRow)
                    {
                        for (int kernelCol = 0; kernelCol < kernel.Width; ++kernelCol)
                        {
                            value += kernel[kernelRow, kernelCol] * matrix[row + kernelRow, col + kernelCol];
                        }
                    }
                    result[row, col] = value;
                }
            }
            return result;
        }

        public static float MaxNeg(Matrix matrix)
        {
            float max = 0;
            for (int row = 0; row < matrix.Height; ++row)
            {
                for (int col = 0; col < matrix.Width; ++col)
                {
                    if (matrix[row, col] < 0)
                    {
                        max = System.Math.Max(max, -matrix[row, col]);
                    }
                }
            }
            return max;
        }

        public static float Max(Matrix matrix)
        {
            float max = 0;
            for (int row = 0; row < matrix.Height; ++row)
            {
                for (int col = 0; col < matrix.Width; ++col)
                {
                    max = System.Math.Max(max, matrix[row, col]);
                }
            }
            return max;
        }

        public static Matrix MultElements(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Height, m1.Width);
            for (int row = 0; row < m1.Height; ++row)
            {
                for (int col = 0; col < m1.Width; ++col)
                {
                    result[row, col] = m1[row, col] * m2[row, col];
                }
            }
            return result;
        }
    }
}
