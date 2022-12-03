using System;
using System.Linq;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        [JsonIgnore]
        public Player Player { get; }

        [JsonIgnore]
        public Thief Thief { get; }

        [JsonIgnore]
        public IInputService Input { get; private set; }

        [JsonIgnore]
        public IOutputService Output { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; private set; }

        public Game(World world, string startingLocation, string startingWeapon, int maxHealth)
        {
            World = world;
            Player = new Player(World, startingLocation, startingWeapon, maxHealth);
            Thief = new Thief(World);
        }

        public void Run(IInputService input, IOutputService output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));

            IsRunning = true;
            Input.InputReceived += OnInputReceived;
            Output.WriteLine("Welcome to Zork!");
            Look();
            Console.WriteLine($"\n{Player.CurrentRoom}");
            Console.WriteLine($"Thief says hi from {Thief.CurrentRoom}");
        }

        public void OnInputReceived(object sender, string inputString)
        {
            char separator = ' ';
            string[] commandTokens = inputString.Split(separator);

            string verb;
            string subject = null;
            if (commandTokens.Length == 0)
            {
                return;
            }
            else if (commandTokens.Length == 1)
            {
                verb = commandTokens[0];
            }
            else
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
            }

            Room previousRoom = Player.CurrentRoom;
            Commands command = ToCommand(verb);
            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    Output.WriteLine("Thank you for playing!");
                    break;

                case Commands.Look:
                    Look();
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = (Directions)command;
                    Output.WriteLine(Player.Move(direction) ? $"You moved {direction}." : "The way is shut!");
                    Thief.ChangeRoom();
                    ThiefStoleItem();
                    ThiefDropItem();
                    Console.WriteLine($"Thief says hi from {Thief.CurrentRoom}");
                    break;

                case Commands.Take:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Take(subject);
                    }
                    break;

                case Commands.Drop:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Drop(subject);
                    }
                    break;

                case Commands.Inventory:
                    if (Player.Inventory.Count() == 0)
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    else
                    {
                        Output.WriteLine("You are carrying:");
                        foreach (Item item in Player.Inventory)
                        {
                            Output.WriteLine(item.Name);
                        }
                    }
                    break;

                case Commands.Reward:
                    Output.WriteLine($"Increased score.");
                    Player.Score++;
                    break;

                case Commands.Score:
                    Output.WriteLine($"Your score is {Player.Score} in {Player.Moves} moves.");
                    break;

                case Commands.Diagnose:
                    Diagnose();
                    break;
                   
                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }

            if(command != Commands.Unknown)
            {
                Player.Moves++;
            }

            if (ReferenceEquals(previousRoom, Player.CurrentRoom) == false)
            {
                Output.WriteLine($"\n{Player.CurrentRoom}");
                Look();
            }

            if(Player.CurrentRoom == Thief.CurrentRoom)
            {
                //ThiefStoleItem();
            }

           Console.WriteLine($"\n{Player.CurrentRoom}");
        }
        
        private void Look()
        {
            Output.WriteLine(Player.CurrentRoom.Description);

            foreach (Item item in Player.CurrentRoom.Inventory)
            {
                Output.WriteLine(item.LookDescription);
            }
        }

        private void Take(string itemName)
        {
            Item itemToTake = Player.CurrentRoom.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToTake == null)
            {
                Output.WriteLine("You can't see any such thing.");                
            }
            else
            {
                Player.AddItemToInventory(itemToTake);
                Player.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine("Taken.");
            }
        }

        private void Drop(string itemName)
        {
            Item itemToDrop = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToDrop == null)
            {
                Output.WriteLine("You can't see any such thing.");                
            }
            else
            {
                Player.CurrentRoom.AddItemToInventory(itemToDrop);
                Player.RemoveItemFromInventory(itemToDrop);
                Output.WriteLine("Dropped.");
            }
        }

        private void ThiefStoleItem()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Thief.CurrentRoom.Inventory.Count());
            Item itemToTake = null;

            if (Thief.CurrentRoom.Inventory.Count() != 0)
            {
                itemToTake = Thief.CurrentRoom.Inventory.ElementAt(rndItem);
            }

            if (itemToTake == null)
            {
                // Output.WriteLine($"The thief tried to take something, but it doesn't exist.");
            }
            else
            {
                Thief.AddItemToInventory(itemToTake);
                Thief.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine($"{itemToTake.Name} was taken.");
            }
        }

        private void ThiefDropItem()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Thief.Inventory.Count());
            Item itemToDrop = null;

            if (Thief.Inventory.Count() != 0)
            {
                itemToDrop = Thief.Inventory.ElementAt(rndItem);
            }

            if (itemToDrop == null)
            {
                //Output.WriteLine($"The thief tried to drop something, but it doesn't exist.");
            }
            else if (itemToDrop.IsValuable == false)
            {
                Thief.RemoveItemFromInventory(itemToDrop);
                Thief.CurrentRoom.AddItemToInventory(itemToDrop);
                Output.WriteLine($"{itemToDrop.Name} was left in {Thief.CurrentRoom}");
            }
        }

        private void Steal()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Player.Inventory.Count());
            Item itemToTake = null;

            if (Player.Inventory.Count() != 0)
            {
                itemToTake = Player.Inventory.ElementAt(rndItem);
            }

            if (itemToTake == null)
            {
                Output.WriteLine($"The thief tried to take something from, but is disappointed by your empty pockets.");
            }
            else
            {
                Thief.AddItemToInventory(itemToTake);
                Player.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine($"Before you knew it, {itemToTake.Name} was stolen!");
            }
        }

        private void ForceDrop()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Player.Inventory.Count());
            Item itemToDrop = null;

            if (Player.Inventory.Count() != 0)
            {
                itemToDrop = Thief.Inventory.ElementAt(rndItem);
            }

            if (itemToDrop == null)
            {
                Output.WriteLine($"The thief gave a nasty kick. If you were holding something, you would have dropped it.");
            }
            else
            {
                Player.RemoveItemFromInventory(itemToDrop);
                Output.WriteLine("The thief gave a nasty kick. You dropped {itemToDrop.Name}!");
            }
        }

        private void Diagnose()
        {

           if(Player.CurrentHealth == Player.MaxHealth)
           {
               Output.WriteLine("You are perfectly healthy.");
           }
           else if(Player.CurrentHealth <= 0)
            {
               Output.WriteLine("You are dead.");
            }
            else
            {
               Output.WriteLine("You are wounded.");
            }

        }

        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}