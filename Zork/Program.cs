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
                Console.Write($"{_rooms[Location.Row, Location.Column]}\n> ");
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
                        if(Move(command))
                        {
                            outputString = $"You moved {command}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
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

        private static bool Move(Commands command)
        {
            bool didMove = false;

            switch (command)
            {
                case Commands.North when Location.Row < _rooms.GetLength(0)-1: 
                    Location.Row++;
                    didMove = true;
                    break;

                case Commands.South when Location.Row > 0:
                    Location.Row--;
                    didMove = true;
                    break;

                case Commands.East when Location.Column < _rooms.GetLength(1) - 1: 
                    Location.Column++;
                    didMove = true;       
                    break;

                case Commands.West when Location.Column > 0:
                    Location.Column--;
                    didMove = true;
                    break;
            }

            return didMove;
        }
        private static readonly string[,] _rooms = {
            {"Rocky Trail", "South of House", "Canyon View" },
            {"Forest", "West of House", "Behind House" },
            {"Dense Woods", "North of House", "Canyon" }
        };

        private static (int Row, int Column) Location = (1, 1);
    }
}