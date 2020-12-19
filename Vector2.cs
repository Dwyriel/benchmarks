using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Benchmarks
{
    public class Vector2
    {
        public float x;
        public float y;
        public static readonly Vector2 up = new Vector2(0, 1);
        public static readonly Vector2 left = new Vector2(-1, 0);
        public static readonly Vector2 right = new Vector2(1, 0);
        public static readonly Vector2 down = new Vector2(0, -1);

        public Vector2()
        {
            this.x = 0;
            this.y = 0;
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "{x:" + x + " y:" + y +"}";
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2)) return false;

            return Equals((Vector2)other);
        }

        public bool Equals(Vector2 other)
        {
            return x == other.x && y == other.y;
        }

        public static float Distance(Vector2 from, Vector2 to)
        {
            return MathF.Sqrt(MathF.Pow(Math.Abs(from.x - to.x), 2) + MathF.Pow(Math.Abs(from.y - to.y), 2));
        }

        private static Vector2 Benchtest1(float signedAngle)
        {
            return (signedAngle > 0) ? (Math.Abs(signedAngle) < 22.5f) ? Vector2.up : (Math.Abs(signedAngle) < 67.5f) ? new Vector2(-.7f, .7f) : (Math.Abs(signedAngle) < 112.5f) ? Vector2.left : (Math.Abs(signedAngle) < 157.5f) ? new Vector2(-.7f, -.7f) : Vector2.down : (Math.Abs(signedAngle) < 22.5f) ? Vector2.up : (Math.Abs(signedAngle) < 67.5f) ? new Vector2(.7f, .7f) : (Math.Abs(signedAngle) < 112.5f) ? Vector2.right : (Math.Abs(signedAngle) < 157.5f) ? new Vector2(.7f, -.7f) : Vector2.down;
        }

        private static Vector2 Benchtest2(float signedAngle)
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

        public static void Benchmark(int threadCount)
        {
            List<Thread> threads = new List<Thread>();
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
            BigInteger iterationsPerThread = iterations / threadCount;
            Console.WriteLine("iterations = " + iterations + " iterations per thread = " + iterationsPerThread);
            for (int count = 0; count < threadCount; count++)
            {
                Thread newThread = new Thread(() =>
                {
                    Thread.Sleep(1000);
                    Stopwatch s = new Stopwatch();
                    BigInteger i = 0;
                    int totalThreadIterations = 0;
                    while (i < iterationsPerThread)
                    {
                        i++;
                        signedAngle = ((float)rng.NextDouble() * 360f) - 180f;
                        Console.WriteLine("Angle: " + signedAngle);
                        s.Restart();
                        for (BigInteger b = 0; b < iterationsPerTest; b++)
                        {
                            instPlace = Benchtest1(signedAngle);
                            totalThreadIterations++;
                        }
                        s.Stop();
                        ternary.Add(s.ElapsedMilliseconds);
                        Console.WriteLine(s.ElapsedMilliseconds);
                        s.Restart();
                        for (BigInteger a = 0; a < iterationsPerTest; a++)
                        {
                            instPlace = Benchtest2(signedAngle);
                            totalThreadIterations++;
                        }
                        s.Stop();
                        ifelse.Add(s.ElapsedMilliseconds);
                        Console.WriteLine(s.ElapsedMilliseconds);
                        Console.WriteLine();
                    }
                    Console.WriteLine("Inverting");
                    Console.WriteLine();
                    Thread.Sleep(1000);
                    i = 0;
                    while (i < iterationsPerThread)
                    {
                        i++;
                        signedAngle = ((float)rng.NextDouble() * 360f) - 180f;
                        Console.WriteLine("Angle: " + signedAngle);
                        s.Restart();
                        for (BigInteger b = 0; b < iterationsPerTest; b++)
                        {
                            instPlace = Benchtest2(signedAngle);
                            totalThreadIterations++;
                        }
                        s.Stop();
                        ifelse.Add(s.ElapsedMilliseconds);
                        Console.WriteLine(s.ElapsedMilliseconds);
                        s.Restart();
                        for (BigInteger a = 0; a < iterationsPerTest; a++)
                        {
                            instPlace = Benchtest1(signedAngle);
                            totalThreadIterations++;
                        }
                        s.Stop();
                        ternary.Add(s.ElapsedMilliseconds);
                        Console.WriteLine(s.ElapsedMilliseconds);
                        Console.WriteLine();
                    }
                    totalIterations += totalThreadIterations;
                });
                newThread.Name = "Thread - " + count;
                threads.Add(newThread);
                newThread.Start();
                Console.WriteLine(newThread.Name + " starting");
            }
            foreach (Thread t in threads)
            {
                t.Join();
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

    public class Vector2Int
    {
        public int x;
        public int y;
        public Vector2Int()
        {
            this.x = 0;
            this.y = 0;
        }
        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "{x:" + x + " y:" + y + "}";
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2Int)) return false;

            return Equals((Vector2Int)other);
        }

        public bool Equals(Vector2Int other)
        {
            return x == other.x && y == other.y;
        }
    }
}
