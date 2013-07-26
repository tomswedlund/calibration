using System;

namespace Tom.Math
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

        public Matrix(int height, int width)
        {
            this._elements = new float[height, width];
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
    }
}
