using System.Collections.Generic;
using System;

namespace Zork
{
    internal class Game
    {
        public World World { get; set; }

        public Player Player { get; set; }

        public void Run()
        {
            InitializeRoomDescription();

                Room previousRoom = null;
                Commands command = Commands.Unknown;
                bool isRunning = true;
                while (isRunning)
                {
                    Console.WriteLine(Player.CurrentRoom);
                    if (previousRoom != Player.CurrentRoom && Player.CurrentRoom.HasBeenVisited == false)
                    {
                        Console.WriteLine(Player.CurrentRoom.Description);
                        previousRoom = Player.CurrentRoom;
                        Player.CurrentRoom.HasBeenVisited = true;
                    }
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
                            outputString = Player.CurrentRoom.Description;
                            break;

                        case Commands.North:
                        case Commands.South:
                        case Commands.East:
                        case Commands.West:
                            if (Player.Move(command))
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


                private static Commands ToCommand(string commandString)
                {
                    return Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
                }

             
              private void InitializeRoomDescription()
                {
                    var roomMap = new Dictionary<string, Room>();
                    foreach (Room room in _rooms)
                    {
                        roomMap[room.Name] = room;
                    }
                    roomMap["Rocky Trail"].Description = "You are on a rock strewn trail.";
                    roomMap["South of House"].Description = "You are facing the south side of a white house. There is no door here, and all the windows are barred.";
                    roomMap["Canyon View"].Description = "You are at the top of the Great Canyon on its south wall.";

                    roomMap["Forest"].Description = "This is a forest, with trees in all directions around you.";
                    roomMap["West of House"].Description = "This is an open field west of a white house, with a boarded front door.";
                    roomMap["Behind House"].Description = "You are behind the white house. In one corner of the house is a small window which is slightly ajar.";

                    roomMap["Dense Woods"].Description = "This is a dimly lit forest, with large trees all around. To the east, there appears to be sunlight.";
                    roomMap["North of House"].Description = "You are facing the north side of a white house. There is no door here, and all the windows are barred.";
                    roomMap["Clearing"].Description = "You are in a clearing, with a forest surrounding you on the west and south.";
                }
                private static bool IsDirection(Commands command) => Directions.Contains(command);

                  private static readonly List<Commands> Directions = new List<Commands>
                  {
                      Commands.North,
                      Commands.South,
                      Commands.East,
                      Commands.West
                  };

            private static (int Row, int Column) _location = (1, 1);
    }
    }
}
