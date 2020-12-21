using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Math
{
    public static class Mathf
    {
        public static float e = 2.71828f;
        public static float Abs(float a)
        {
            return (a < 0f) ? (a * -1) : a;
        }

        public static float Pow(float a, float b)
        {
            return (float)System.Math.Exp(b * System.Math.Log(a));
        }

        public static float Sqrt(float a)
        {
            return Mathf.Pow(a, .5f);
        }

        public static float Root(float a, float b)
        {
            return Pow(a, (1.0f / b));
        }
    }
}
