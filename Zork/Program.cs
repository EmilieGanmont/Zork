using System;
using System.ComponentModel;

namespace Zork
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");

            Commands command = Commands.Unknown;
            bool isRunning = true;

            while (isRunning)
            {
                Console.Write("> ");
                command = ToCommand(Console.ReadLine().Trim());

                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.Look:
                        outputString = "This is an open field west of a white house, with a boarded front door. \nA rubber mat saying 'Welcome to Zork!' lies by the door.";
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        outputString = $"You moved {command}.";
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }

                Console.WriteLine(outputString);
            }
        }

        static Commands ToCommand(string commandString)
        {
            return Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
          
        }

        private static readonly string[] _rooms = { "Forest", "West of House", "Behind House", "Clearing", "Canyon View" };
        private static int _currentRoom = 1;
    }
}