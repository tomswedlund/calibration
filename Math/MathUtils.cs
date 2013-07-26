using System;

namespace Tom.Math
{
    public static class MathUtils
    {
        public static bool SolveQuadratic(float A, float B, float C, out float plus, out float minus)
        {
            float root = B * B - 4 * A * C;
            if (root < 0)
            {
                plus = float.NaN;
                minus = float.NaN;
                return false;
            }

            float denom = 2 * A;
            if (IsZero(denom))
            {
                plus = float.NaN;
                minus = float.NaN;
                return false;
            }

            float sqrt = (float)System.Math.Sqrt(root);
            plus = (-B + sqrt) / denom;
            minus = (-B - sqrt) / denom;
            return true;
        }

        public static bool IsZero(float value)
        {
            return (System.Math.Abs(value) < 0.0001f);
        }
    }
}
