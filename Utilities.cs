

using System.Collections.Generic;

namespace CustomDRP
{
    internal class Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        { 
            X = x;
            Y = y;
        }
    }

    internal class Effect
    {
        public static float Linear(float x, float y)
        {
            if (x <= y) { return x; }
            else { return y; }
        }

        public static float Racing(float x, float y)
        {
            float _returned = 2 * x;

            if (_returned <= y) { return _returned; }
            else { return y; }
        }
    }

    internal class Other
    {
        public static float Limit(float x, float z, float y)
        {
            if (x <= y && x >= z) { return x; }
            else if (x > y) { return y; }
            else { return z; }
        }

        public static float Limit(float x, float z)
        {
            if (x >= z) { return x; }
            else { return z; }
        }
    }
}
