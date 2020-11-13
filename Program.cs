using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace TestVector2Speed
{
    class Program
    {
        public static Vector2 Benchtest1(float signedAngle)
        {
            return (signedAngle > 0) ? (Math.Abs(signedAngle) < 22.5f) ? Vector2.up : (Math.Abs(signedAngle) < 67.5f) ? new Vector2(-.7f, .7f) : (Math.Abs(signedAngle) < 112.5f) ? Vector2.left : (Math.Abs(signedAngle) < 157.5f) ? new Vector2(-.7f, -.7f) : Vector2.down : (Math.Abs(signedAngle) < 22.5f) ? Vector2.up : (Math.Abs(signedAngle) < 67.5f) ? new Vector2(.7f, .7f) : (Math.Abs(signedAngle) < 112.5f) ? Vector2.right : (Math.Abs(signedAngle) < 157.5f) ? new Vector2(.7f, -.7f) : Vector2.down;
        }

        public static Vector2 Benchtest2(float signedAngle)
        {
            Vector2 instPlace;
            if (Math.Abs(signedAngle) > 22.5f && Math.Abs(signedAngle) < 67.5f)
                instPlace = (signedAngle > 0) ? new Vector2(-.7f, .7f) : new Vector2(.7f, .7f);
            else if (Math.Abs(signedAngle) > 67.5f && Math.Abs(signedAngle) < 112.5f)
                instPlace = (signedAngle > 0) ? Vector2.left : Vector2.right;
            else if (Math.Abs(signedAngle) > 112.5f && Math.Abs(signedAngle) < 157.5f)
                instPlace = (signedAngle > 0) ? new Vector2(-.7f, -.7f) : new Vector2(.7f, -.7f);
            else
                instPlace = (Math.Abs(signedAngle) > 90) ? Vector2.down : Vector2.up;
            return instPlace;
        }

        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            // System.Diagnostics Stopwatch
            Random rng = new Random();
            Vector2 instPlace;
            float signedAngle;
            List<BigInteger> ternary = new List<BigInteger>();
            List<BigInteger> ifelse = new List<BigInteger>();
            Console.WriteLine("How many iterations per type of test? (int)");
            BigInteger iterationsPerTest = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("How many total iterations? (int)");
            BigInteger iterations = BigInteger.Parse(Console.ReadLine());
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            BigInteger totalIterations = 0;
            BigInteger i = 0;
            while (i < iterations)
            {
                i++;
                signedAngle = ((float)rng.NextDouble() * 360f) - 180f;
                Console.WriteLine("Angle: " + signedAngle);
                s.Restart();
                for (BigInteger b = 0; b < iterationsPerTest; b++)
                {
                    instPlace = Benchtest1(signedAngle);
                    totalIterations++;
                }
                s.Stop();
                ternary.Add(s.ElapsedMilliseconds);
                Console.WriteLine(s.ElapsedMilliseconds);
                s.Restart();
                for (BigInteger a = 0; a < iterationsPerTest; a++)
                {
                    instPlace = Benchtest2(signedAngle);
                    totalIterations++;
                }
                s.Stop();
                ifelse.Add(s.ElapsedMilliseconds);
                Console.WriteLine(s.ElapsedMilliseconds);
                Console.WriteLine();
            }
            Console.WriteLine("Inverting");
            Console.WriteLine();
            System.Threading.Thread.Sleep(4000);
            i = 0;
            while (i < iterations)
            {
                i++;
                signedAngle = ((float)rng.NextDouble() * 360f) - 180f;
                Console.WriteLine("Angle: " + signedAngle);
                s.Restart();
                for (BigInteger b = 0; b < iterationsPerTest; b++)
                {
                    instPlace = Benchtest2(signedAngle);
                    totalIterations++;
                }
                s.Stop();
                ifelse.Add(s.ElapsedMilliseconds);
                Console.WriteLine(s.ElapsedMilliseconds);
                s.Restart();
                for (BigInteger a = 0; a < iterationsPerTest; a++)
                {
                    instPlace = Benchtest1(signedAngle);
                    totalIterations++;
                }
                s.Stop();
                ternary.Add(s.ElapsedMilliseconds);
                Console.WriteLine(s.ElapsedMilliseconds);
                Console.WriteLine();
            }
            BigInteger total = 0;
            foreach (BigInteger value in ternary)
            {
                total += value;
            }
            Console.WriteLine("Ternary avg = " + (total / ternary.Count));
            total = 0;
            foreach (BigInteger value in ifelse)
            {
                total += value;
            }
            Console.WriteLine("Ifelse avg = " + (total / ifelse.Count));
            Console.WriteLine("total iterations = " + totalIterations);
            Console.ReadLine();
        }
    }

    public class Vector2
    {
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float x;
        public float y;
        public static Vector2 up = new Vector2(0, 1);
        public static Vector2 left = new Vector2(-1, 0);
        public static Vector2 right = new Vector2(1, 0);
        public static Vector2 down = new Vector2(0, -1);
    }
}


