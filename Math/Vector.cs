namespace Tom.Math
{
    public class Vector
    {
        private float[] _elements = null;

        public float Length
        {
            get { return this._elements.Length; }
        }

        public float this[int index]
        {
            get { return this._elements[index]; }
            set { this._elements[index] = value; }
        }

        public Vector(int length)
        {
            this._elements = new float[length];
        }
    }
}
