﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalProgramming
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // var numbers = new[] { 3, 5, 7, 9, 11, 13 };
            // using GetLazyRandomNumber() now for a stream of numbers
            foreach (var prime in GetLazyRandomNumber(100).Find(IsPrime).Take(2))
            {
                Console.WriteLine("Prime number found: {0}", prime);
            }
        }

        private static IEnumerable<int> GetLazyRandomNumber(int max)
        {
            var rand = new Random();
            while (true)
            {
                yield return rand.Next(max);
            }

        }

        // Find() is a lazy, higher order function, yield returning one result at a time, 
        // saving processing time.
        // Note that it takes a Func<> as a paramater - this can be any named or anonymous function.
        private static IEnumerable<int> Find(this IEnumerable<int> values, Func<int, bool> test)
        {
            foreach (var number in values)
            {
                Console.WriteLine("Testing value {0}", number);
                if (test(number))
                {
                    yield return number;
                }
            }
        }

        private static bool IsPrime(int number)
        {
            bool result = true;
            for (long i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    result = false;
                    break;

                }
            }
            return result;
        }
    }
}