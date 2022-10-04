using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace Zork
{
    class Program
    {
        static void Main(string[] args)
        {
            const string defaultGameFilename = @"Content/Zork.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultGameFilename);

            Game game = Game.Load(gameFilename);
            Console.WriteLine("Welcome to Zork!");
            game.Run();
            Console.WriteLine("Thank you for playing!");
        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}