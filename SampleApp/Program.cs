using System;

namespace SampleApp.App;

internal class Program
{
    static void Main(string[] args)
    {
        new AppModule().Build();
        Console.WriteLine("Hello, World!");
    }
}