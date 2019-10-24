using Microsoft.JSInterop;
using System;

namespace BookkeepingNasheDetstvo.Client.Extensions
{
    internal static class LoggerExtensions
    {
        internal static void Log(string message)
        {
            Console.WriteLine(message);
        }

        internal static void Log(object obj)
        {
            Console.WriteLine(Json.Serialize(obj));
        }
    }
}
