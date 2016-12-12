using System;
using PureConfig;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser pure = new Parser("test.pure");
            string message = pure.Get<string>("hello");
            Console.WriteLine(message);
        }
    }
}
