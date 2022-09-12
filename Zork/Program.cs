using System;
using System.ComponentModel;

namespace Zork
{
    class Program
    {
        private static Room CurrentRoom
        {
            get
            {
                return _rooms[_location.Row, _location.Column];
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");

            Commands command = Commands.Unknown;
            bool isRunning = true;

            while (isRunning)
            {
                Console.Write($"{CurrentRoom}\n> ");
                command = ToCommand(Console.ReadLine().Trim());

                string outputString;
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.Look:
                        outputString = CurrentRoom.Description;
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
                case Commands.North when _location.Row < _rooms.GetLength(0) - 1: 
                    _location.Row++;
                    didMove = true;
                    break;

                case Commands.South when _location.Row > 0:
                    _location.Row--;
                    didMove = true;
                    break;

                case Commands.East when _location.Column < _rooms.GetLength(1) - 1: 
                    _location.Column++;
                    didMove = true;       
                    break;

                case Commands.West when _location.Column > 0:
                    _location.Column--;
                    didMove = true;
                    break;
            }

            return didMove;
        }

        private static void InitializeRoomDescription()
        {
            _rooms[0, 0].Description = "Rock";
            _rooms[0, 1].Description = "South side";
            _rooms[0, 2].Description = "Canyon";

            _rooms[1, 0].Description = "a";
            _rooms[1, 1].Description = "a";
            _rooms[1, 2].Description = "a";

            _rooms[2, 0].Description = "a";
            _rooms[2, 1].Description = "a";
            _rooms[2, 2].Description = "a";
        }

        private static readonly Room[,] _rooms = {
            {new Room("Rocky Trail"), new Room("South of House"), new Room("Canyon View") },
            {new Room("Forest"), new Room("West of House"), new Room("Behind House") },
            {new Room("Dense Woods"), new Room("North of House"), new Room("Clearing") }
        };

        private static (int Row, int Column) _location = (1, 1);
    }

}