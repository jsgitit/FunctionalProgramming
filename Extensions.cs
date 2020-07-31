using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace FunctionalProgramming
{
    public static class Extensions
    {
        public static T WithRetry<T>(this Func<T> action)
        {
            var result = default(T);
            var retryCount = 0;
            var success = false;
            
            do
            {
                try
                {
                    result = action();
                    success = true;
                }
                catch (WebException ex)
                {
                    retryCount++;
                }
            } while (retryCount < 3 && !success);
            return result;
        }

        // Partial application function.    Transform a function that takes
        // more parameters and turn it into one that takes less parameters.
        public static Func<TResult> Partial<TParam1, TResult>(this Func<TParam1, TResult> func, TParam1 parameter)
        {
            return () => func(parameter);
        }

        // Curry function
        public static Func<TParam1, Func<TResult>> Curry<TParam1, TResult> (this Func<TParam1, TResult> func)
        {
            return parameter => () => func(parameter);
        }
    }
}
