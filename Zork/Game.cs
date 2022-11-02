using System;
using System.IO;
using Newtonsoft.Json;

namespace Zork
{
    public class Game
    {
        public World World { get; private set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; set; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;
        }

        public void Run()
        {
            IsRunning = true;
            Room previousRoom = null;
            while (IsRunning)
            {
                Console.Write(Player.Location);
                if (previousRoom != Player.Location)
                {
                    Console.WriteLine(Player.Location.Description);
                    previousRoom = Player.Location;
                    ShowItems();
                }

                Console.Write("\n> ");

                string inputString = Console.ReadLine().Trim();
                const char seperator = ' ';
                string[] commandTokens = inputString.Split(seperator);

                string verb = null;
                string subject = null;

                Commands command = Commands.Unknown;

                switch (commandTokens.Length)
                {
                    case 0:
                        continue;

                    case 1:
                        verb = commandTokens[0];
                        command = ToCommand(verb);
                        break;

                    case 2:
                        verb = commandTokens[0];
                        subject = commandTokens[1];

                        command = ToCommand(verb);
                        break;

                }

                switch (command)
                {
                    case Commands.Quit:
                        IsRunning = false;
                        break;

                    case Commands.Look:
                        Console.WriteLine(Player.Location.Description);
                        ShowItems();
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = Enum.Parse<Directions>(command.ToString(), true);
                        if (Player.Move(direction) == false)
                        {
                            Console.WriteLine("The way is shut!");
                        }
                        break;
                    case Commands.Inventory:
                        if (Player.Inventory.Count <= 0)
                        {
                            Console.WriteLine("You are empty handed.");
                        }
                        else if (Player.Inventory.Count > 0)
                        {
                            foreach (Item i in Player.Inventory)
                            {
                                Console.WriteLine(i.Description);
                            }
                        }
                        break;
                    case Commands.Take:
                        if (subject != null)
                        {
                            foreach (Item i in Player.Location.Inventory)
                            {
                                if (subject.Equals(i.Name))
                                {
                                    Player.Inventory.Add(i);
                                    Player.Location.Inventory.Remove(i);

                                    Console.WriteLine("Taken.");
                                    break;
                                }
                                else if(subject.Equals(i.Name) != true)
                                {
                                    Console.WriteLine("There is no such thing");
                                    break;
                                }
                            }
                        }
                        else if (subject == null)
                        {
                            Console.WriteLine("Take what?");
                        }
                        break;

                    case Commands.Drop:
                        if (subject != null)
                        {
                            foreach (Item i in Player.Inventory)
                            {
                                if (subject.Equals(i.Name))
                                {
                                    Player.Inventory.Remove(i);
                                    Player.Location.Inventory.Add(i);

                                    Console.WriteLine("Dropped.");
                                    Console.Write("\n> ");
                                    break;
                                }
                                else if(subject.Equals(i.Name) == false)
                                {
                                    Console.WriteLine("You don't have that thing");
                                    break;
                                }
                            }
                        }
                        else if (subject == null)
                        {
                            Console.WriteLine("Drop what?");
                            break;
                        }

                        if(Player.Inventory.Count == 0 )
                        {
                            Console.WriteLine("You don't have that thing.");
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown command");
                        break;

                }

                Console.Write("\n");
            }
        }

        public static Game Load(string filename)
        {
            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(filename));
            game.Player = game.World.SpawnPlayer();
            return game;
        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;

        private void ShowItems()
        {
            foreach (Item i in Player.Location.Inventory)
            {
                Console.WriteLine(i.Description);
            }
        }

    }
}
