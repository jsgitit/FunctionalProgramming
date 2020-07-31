using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FunctionalProgramming
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var timekeeper = new Timekeeper();  // Timekeeper allows us to measure how long an action takes to run

            var elapsed = timekeeper.Measure(() =>
            {
                // var numbers = new[] { 3, 5, 7, 9, 11, 13 };
                // using GetLazyRandomNumber() now for a stream of numbers
                foreach (var prime in GetLazyRandomNumber(100).Find(IsPrime).Take(2))
                {
                    Console.WriteLine("Prime number found: {0}", prime);
                }
            });
            Console.WriteLine("The elapsed time was {0}", elapsed);
            Console.ReadLine();
            // Partial example

            var client = new WebClient();
            Func<string, string> download = url => client.DownloadString(url);

            var data = download.Partial("https://api.tlopo.com/system/status/").WithRetry();
            Console.WriteLine("Using Partial function with retry returns the following string: \nn" + data);
            Console.ReadLine();
            // Curry example
            // build a Func of string that returns a func of string
            Func<string, Func<string>> downloadCurry = download.Curry();
            var data2 = downloadCurry("https://api.tlopo.com/system/status/").WithRetry();
            Console.WriteLine("Using Curry function with retry returns the following string: \n\n" + data2);

            // Now let's use TPL Async and Task in functional programming.

            elapsed = timekeeper.Measure(() => FindLargePrimes(900000, 1000000));
            Console.WriteLine("Finding all prime numbers took: {0}", elapsed);

            // now use Async()
            elapsed = timekeeper.Measure(() => FindLargePrimesAsynch(900000, 1000000));
            Console.WriteLine("Finding all prime numbers in parallel took: {0}", elapsed);

            // now use Task
            var task = new Task<IEnumerable<int>>(() => FindLargePrimes(3, 100));
            var task2 = task.ContinueWith((prev) =>
            {
                foreach (var number in prev.Result)
                {
                    Console.WriteLine("The number found was {0}", number);

                }
            });
            task.Start();
            Console.WriteLine("Doing some other work right now");

            task2.Wait(); 

        }

        private static IList<int> FindLargePrimes(int start, int end)
        {
            var primes = Enumerable.Range(start, end - start).ToList();
            return primes.Where(IsPrime).ToList();
        }
        private static IList<int> FindLargePrimesAsynch(int start, int end)
        {
            var primes = Enumerable.Range(start, end - start).ToList();
            return primes.AsParallel().Where(IsPrime).ToList();
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

        public class Timekeeper
        {
            // Measure() takes a "Action" as a parameter and measures the 
            // elapsed time to run the function.
            public TimeSpan Measure(Action action)
            {
                var watch = new Stopwatch();
                watch.Start();
                action();
                return watch.Elapsed;
            }
        }


    }
}