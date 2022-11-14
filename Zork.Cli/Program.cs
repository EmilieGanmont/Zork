using System;
using Zork.Common;

namespace Zork.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            const string defaultGameFilename = @"Content/Zork.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultGameFilename);
            Game game = Game.Load(gameFilename);

            var output = new ConsoleOutputService();
            var input = new ConsoleInputService();

            Console.WriteLine("Welcome to Zork!");
            game.Run(input, output);

            //game.Player.MovesChanged += Player_MovesChanged;

            while(game.IsRunning)
            {
                game.Output.Write("> ");
                input.ProcessInput();
            }

            game.Output.WriteLine("Thank you for playing!");
        }

       //private static void Player_MovesChanged(object sender, int moves)
       //{
       //    game.Output.WriteLine($"You've made {moves} moves.");
       //}

        private static void Input_InputReceived(object sender, string inputString)
        {
            Console.WriteLine(inputString);
        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}