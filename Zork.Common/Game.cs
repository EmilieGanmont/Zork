using System;
using System.IO;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; private set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        public IInputService Input { get; private set; }
        public IOutputService Output { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; private set; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;
        }

        public void Run(IInputService input, IOutputService output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));

            Input.InputReceived += Input_InputReceived;
            IsRunning = true;

            Output.WriteLine(Player.Location);
            Output.WriteLine(Player.Location.Description);
            ShowItems();
            Output.Write("\n");
        }

        private void Input_InputReceived(object sender, string inputString)
        {
            const char seperator = ' ';
            string[] commandTokens = inputString.Split(seperator);

            string verb = null;
            string subject = null;

            Room previousRoom = Player.Location;

            Commands command = Commands.Unknown;

            switch (commandTokens.Length)
            {
                case 0:
                    break;

                case 1:
                    verb = commandTokens[0];
                    command = ToCommand(verb);
                    break;

                case 2:
                    verb = commandTokens[0];
                    subject = commandTokens[1];

                    command = ToCommand(verb);
                    break;

                default:
                    Output.WriteLine("Try a simpler command.");
                    Output.Write("\n");
                    break;
            }

            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    break;

                case Commands.Look:
                    Output.WriteLine(Player.Location.Description);
                    ShowItems();
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = Enum.Parse<Directions>(command.ToString(), true);
                    if (Player.Move(direction) == false)
                    {
                        Output.WriteLine("The way is shut!");
                    }
                    break;

                case Commands.Inventory:
                    if (Player.Inventory.Count <= 0)
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    else if (Player.Inventory.Count > 0)
                    {
                        Output.WriteLine("You are carrying:");
                        foreach (Item i in Player.Inventory)
                        {
                            Output.WriteLine($"A {i.Name.ToLower()}");
                        }
                    }
                    break;

                case Commands.Take:
                    Item itemToTake = null;
                    if (subject != null)
                    {
                        foreach (Item i in Player.Location.Inventory)
                        {
                            if (string.Compare(subject, i.Name, ignoreCase: true) == 0)
                            {
                                itemToTake = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Output.WriteLine("What do you want to take?");
                        break;
                    }

                    if (itemToTake != null)
                    {
                        Player.Location.Inventory.Remove(itemToTake);
                        Player.Inventory.Add(itemToTake);

                        Output.WriteLine("Taken.");
                    }
                    else
                    {
                        Output.WriteLine("There is no such thing.");
                    }
                    break;

                case Commands.Drop:
                    Item itemToDrop = null;
                    if (subject != null)
                    {
                        foreach (Item i in Player.Inventory)
                        {
                            if (string.Compare(subject, i.Name, ignoreCase: true) == 0)
                            {
                                itemToDrop = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Output.WriteLine("What do you want to drop?");
                        break;
                    }

                    if (itemToDrop != null)
                    {
                        Player.Inventory.Remove(itemToDrop);
                        Player.Location.Inventory.Add(itemToDrop);

                        Output.WriteLine("Dropped.");
                    }
                    else
                    {
                        Output.WriteLine("You don't have that thing");
                    }
                    break;

                default:
                    Output.WriteLine("Unknown command");
                    break;
            }

            if(command !=  Commands.Unknown )
            {
                Player.Moves++;
            }

            if (previousRoom != Player.Location)
            {
                Output.WriteLine(Player.Location);
                Output.WriteLine(Player.Location.Description);
                ShowItems();
            }

            Output.Write("\n");
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
                Output.WriteLine(i.Description);
            }
        }

    }
}
