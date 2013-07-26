using System;
using System.Collections.Generic;

namespace Tom.Math
{
    public class SparseMatrix
    {
        private Dictionary<RowColKey, float> _elements = new Dictionary<RowColKey, float>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public float this[int row, int col]
        {
            get
            {
                ThrowIfOutOfRange(row, col, this.Width, this.Height);
                RowColKey key = new RowColKey(row, col);
                if (this._elements.ContainsKey(key))
                {
                    return this._elements[key];
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                ThrowIfOutOfRange(row, col, this.Width, this.Height);
                RowColKey key = new RowColKey(row, col);
                if (this._elements.ContainsKey(key))
                {
                    this._elements[key] = value;
                }
                else
                {
                    this._elements.Add(key, value);
                }
            }
        }

        public SparseMatrix(int height, int width)
        {
            this.Height = height;
            this.Width = width;
        }

        private void ThrowIfOutOfRange(int row, int col, int width, int height)
        {
            if ((row < 0) || (row >= height) || (col < 0) || (col >= width))
            {
                throw new IndexOutOfRangeException();
            }
        }
    }

    internal class RowColKey
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public RowColKey(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int hash = 1;
            hash = prime * hash + this.Row.GetHashCode();
            hash = prime * hash + this.Col.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (System.Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            RowColKey key = obj as RowColKey;
            if (key == null)
            {
                return false;
            }

            return ((this.Row == key.Row) && (this.Col == key.Col));
        }
    }
}
