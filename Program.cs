﻿using System;
using Benchmarks;

namespace MainProgram
{
    class Program
    {
        static readonly int[] options = { 0, 1, 2 };
        static void Main(string[] args)
        {
            int numOfVirtCores = Environment.ProcessorCount;
            bool wrongInput = true;
            int option;
            do
            {
                Console.WriteLine("Which benchmark do you want to perform:");
                Console.WriteLine("1 - Ternary x Ifelse");
                Console.WriteLine("2 - A* Pathfinding");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("");
                string input = Console.ReadLine();
                if (int.TryParse(input, out option))
                    wrongInput = false;
                foreach (int num in options)
                {
                    if (num == option)
                    {
                        wrongInput = false;
                        break;
                    }
                    else
                        wrongInput = true;
                }
                if (wrongInput)
                    Console.WriteLine("Invalid, try again.\n");
            } while (wrongInput);
            switch (option)
            {
                case 1:
                    Vector2.Benchmark(numOfVirtCores);
                    break;
                case 2:
                    Pathfinding.Benchmark(numOfVirtCores);
                    break;
                case 0:
                    Console.WriteLine("Exiting..");
                    System.Environment.Exit(0);
                    break;
                default:
                    return;
            }
            Console.WriteLine("");
            Console.WriteLine("Enter to exit.");
            Console.ReadLine();
        }
    }
}


