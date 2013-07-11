using System;
namespace calibration
{
    public class Matrix
    {
        private float[,] _elements = null;
        private int _currentInitializerRow = 0;

        public float this[int row, int col]
        {
            get
            {
                return this._elements[row, col];
            }

            set
            {
                this._elements[row, col] = value;
            }
        }

        public int Width
        {
            get { return this._elements.GetLength(1); }
        }

        public int Height
        {
            get { return this._elements.GetLength(0); }
        }

        public Matrix(int width, int height)
        {
            this._elements = new float[width, height];
        }

        public Matrix(int width, int height, float initialValue)
            : this(width, height)
        {
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    this._elements[j, i] = initialValue;
                }
            }
        }

        public Matrix(Matrix mat)
        {
            this._elements = new float[mat.Height, mat.Width];
            for (int i = 0; i < mat.Height; ++i)
            {
                for (int j = 0; j < mat.Width; ++j)
                {
                    this._elements[i, j] = mat[i, j];
                }
            }
        }

        public void Add(params float[] row)
        {
            if (row.Length != this.Width)
            {
                throw new ArgumentException("Row initializer of incorrect bounds.");
            }
            if (this._currentInitializerRow >= this.Height)
            {
                throw new ArgumentException("Cannot add any more rows to matrix.");
            }
            
            for (int i = 0; i < row.Length; ++i)
            {
                this._elements[this._currentInitializerRow, i] = row[i];
            }
            ++this._currentInitializerRow;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if ((m1.Width != m2.Width) || (m1.Height != m2.Height))
            {
                throw new ArgumentException("Matrixes not of equal size.");
            }

            Matrix result = new Matrix(m1);
            for (int row = 0; row < result.Height; ++row)
            {
                for (int col = 0; col < result.Width; ++col)
                {
                    result[row, col] += m2[row, col];
                }
            }
            return result;
        }

        public static Matrix operator *(Matrix mat, float scalar)
        {
            Matrix result = new Matrix(mat);
            for (int row = 0; row < result.Height; ++row)
            {
                for (int col = 0; col < result.Width; ++col)
                {
                    result[row, col] *= scalar;
                }
            }
            return result;
        }

        public static Matrix operator /(Matrix mat, float scalar)
        {
            scalar = 1 / scalar;
            return mat * scalar;
        }

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
                        max = Math.Max(max, -matrix[row, col]);
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
                    max = Math.Max(max, matrix[row, col]);
                }
            }
            return max;
        }
    }
}
