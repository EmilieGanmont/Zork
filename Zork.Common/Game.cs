using System;
using System.Collections.Generic;
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

        public Game(World world, string startingLocation, string startingWeapon, int maxHealth, int thiefDeathChance)
        {
            World = world;
            Player = new Player(World, startingLocation, startingWeapon, maxHealth);
            Thief = new Thief(World, thiefDeathChance);
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
                    ThiefAction();
                    Thief.ChangeRoom();

                    break;

                case Commands.Take:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        if (!Player.IsDead())
                        {
                            Take(subject);
                        }
                        else
                        {
                            Output.WriteLine($"The {subject} passed through your hand.");
                        }

                    }

                    ThiefAction();
                    Thief.ChangeRoom();
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

                    ThiefAction();
                    Thief.ChangeRoom();

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
                    Output.WriteLine($"Great job here's a point.");
                    Player.Score += 5;
                    break;

                case Commands.Score:
                    Output.WriteLine($"Your score is {Player.Score} in {Player.Moves} moves.");
                    break;

                case Commands.Attack:
                    if (Player.PlayerWeapon == null)
                    {
                        Output.WriteLine("You don't have your sword!");
                    }
                    else if (Player.CurrentRoom != Thief.CurrentRoom)
                    {
                        Output.WriteLine("You attack the air because there is no one here.");
                    }
                    else
                    {
                        PlayerAttack();
                    }
                    break;

                case Commands.Diagnose:
                    Diagnose();
                    break;

                case Commands.Resurrection:
                    if (Player.IsDead())
                    {
                        Output.WriteLine($"By praying, you restore to full health.");
                        Player.CurrentHealth = Player.MaxHealth;
                    }
                    else
                    {
                        Output.WriteLine($"You say a prayer, but nothing happened.");
                    }
                    break;

                case Commands.Locate:
                    if (!Thief.IsDead)
                    {
                        Output.WriteLine($"There is a thief in {Thief.CurrentRoom}");
                    }
                    else
                    {
                        Output.WriteLine($"There is a dead thief in {Thief.CurrentRoom}");
                    }
                    break;

                case Commands.Summon:
                    if (!Thief.IsDead)
                    {
                        Output.WriteLine("By making a strange chant, you summon a thief here. He doesn't seem happy too about it.");
                        Thief.CurrentRoom = Player.CurrentRoom;
                    }
                    else
                    {
                        Output.WriteLine("There is no one to summon.");
                    }
                    break;

                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }

            if (command != Commands.Unknown)
            {
                Player.Moves++;
            }

            if (ReferenceEquals(previousRoom, Player.CurrentRoom) == false)
            {
                Output.WriteLine($"\n{Player.CurrentRoom}");
                Look();
            }


            if (Player.CurrentRoom == Thief.CurrentRoom && !Thief.IsDead)
            {
                Output.WriteLine("There is a thief here.");
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

        private void StealFromRoom()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Thief.CurrentRoom.Inventory.Count());
            Item itemToTake = null;

            if (Thief.CurrentRoom.Inventory.Count() != 0)
            {
                itemToTake = Thief.CurrentRoom.Inventory.ElementAt(rndItem);
            }

            if (itemToTake != null)
            {
                if (itemToTake == Thief.CurrentRoom.Inventory.FirstOrDefault(item => string.Compare(item.Name, "sword", ignoreCase: true) == 0))
                {
                    Thief.AddItemToInventory(itemToTake);
                    Thief.CurrentRoom.RemoveItemFromInventory(itemToTake);

                    Output.WriteLine($"Theif stole {itemToTake}");

                    if (Thief.CurrentRoom == Player.CurrentRoom)
                    {
                        Output.WriteLine($"The thief disappeared along with the {itemToTake.Name}...");
                    }
                }
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

             if (itemToDrop != null && itemToDrop.IsValuable == false)
            {
                Thief.RemoveItemFromInventory(itemToDrop);
                Thief.CurrentRoom.AddItemToInventory(itemToDrop);
            }
        }

        private void StealFromPlayer()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Player.Inventory.Count());
            Item itemToTake = null;

            if (Player.Inventory.Count() != 0)
            {
                itemToTake = Player.Inventory.ElementAt(rndItem);
            }

            if (itemToTake == null || itemToTake == Player.PlayerWeapon)
            {
                Output.WriteLine($"The thief tried to take something from you, but is disappointed by your empty pockets.");
            }
            else
            {
                Thief.AddItemToInventory(itemToTake);
                Player.RemoveItemFromInventory(itemToTake);
                Output.WriteLine($"Before you knew it, your {itemToTake.Name} was stolen!");
            }
        }

        private void ForceDrop()
        {
            Random rnd = new Random();
            int rndItem = rnd.Next(Player.Inventory.Count());
            Item itemToDrop = null;

            if (Player.Inventory.Count() != 0)
            {
                itemToDrop = Player.Inventory.ElementAt(rndItem);
            }

            if (itemToDrop == null)
            {
                Output.WriteLine($"The thief gave a nasty kick. If you were holding something, you would have dropped it.");
            }
            else
            {
                Player.RemoveItemFromInventory(itemToDrop);
                Player.CurrentRoom.AddItemToInventory(itemToDrop);
                Output.WriteLine($"The thief gave a nasty kick. You dropped the {itemToDrop.Name}!");
            }
        }

        private void PlayerAttack()
        {
            Output.WriteLine("You attacked the thief!");

            Random rnd = new Random();
            int successChance = 0;


            if (Player.Score > 0)
            {
                successChance = rnd.Next(Player.Score);
            }

            if (successChance >= Thief.DeathChance)
            {
                Output.WriteLine("With a slice to the head, the thief gives his last breath and is enveloped by a black fog.");
                Output.WriteLine("As the fog lifts, all that's left is the thief's spoils on the ground.");

                Thief.IsDead = true;

                foreach (Item i in Thief.Inventory.ToList())
                {
                    Thief.RemoveItemFromInventory(i);
                    Thief.CurrentRoom.AddItemToInventory(i);
                }
            }
            else
            {
                Output.WriteLine("The thief dodged! He stabs you with his stiletto leaving a nasty gash.");
                Player.CurrentHealth--;
                Thief.ChangeRoom();

                if (Player.IsDead())
                {
                    Output.Write("You are now dead. You dropped everything on the ground.");

                    if (Player.Inventory.Count() != 0)
                    {
                        foreach (Item i in Player.Inventory.ToList())
                        {
                            Player.RemoveItemFromInventory(i);
                            Player.CurrentRoom.AddItemToInventory(i);
                        }
                    }

                }
            }

        }

        private void ThiefAction()
        {
            List<Action> thiefActions = new List<Action>();
            thiefActions.Add(ForceDrop);
            thiefActions.Add(StealFromPlayer);

            List<Action> thiefAloneActions = new List<Action>();
            thiefAloneActions.Add(StealFromRoom);
            thiefAloneActions.Add(ThiefDropItem);


            Random rnd = new Random();
            int rndAction = rnd.Next(thiefActions.Count);
            int rndAloneAction = rnd.Next(thiefAloneActions.Count);

            if (!Thief.IsDead)
            {
                if (Player.CurrentRoom == Thief.CurrentRoom)
                {
                    thiefActions.ElementAt(rndAction).Invoke();
                }
                    thiefAloneActions.ElementAt(rndAloneAction).Invoke();
            }
        }

        private void Diagnose()
        {

            if (Player.CurrentHealth == Player.MaxHealth)
            {
                Output.WriteLine("You are perfectly healthy.");
            }
            else if (Player.CurrentHealth <= 0)
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