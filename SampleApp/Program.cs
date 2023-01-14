using System;

namespace SampleApp.App;

internal class Program
{
    static async void Main(string[] args)
    {
        await new AppModule().Build();
        Console.WriteLine("Hello, World!");
    }
}