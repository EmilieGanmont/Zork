using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Player
    {
        public event EventHandler<int> MovesChanged;
        private int _moves;

        public World World { get; }

        [JsonIgnore]
        public Room Location { get; private set; }

        [JsonIgnore]
        public string LocationName
        {
            get
            {
                return Location?.Name;
            }
            set
            {
                Location = World?.RoomsByName.GetValueOrDefault(value);
            }
        }

        public List<Item> Inventory { get; }

        public int Moves
        {
            get
            {
                return _moves;
            }

            set
            {
                if(_moves !=value)
                {
                    _moves = value;
                    MovesChanged?.Invoke(this, _moves);
                }
            }
        }

        [JsonIgnore]
        public Dictionary<string, Item> ItemsByName { get; }

        public Player(World world, string startingLocation)
        {
            World = world;
            LocationName = startingLocation;
            Inventory = new List<Item>();
        }

        public bool Move(Directions direction)
        {
            bool isValidMove = Location.Neighbors.TryGetValue(direction, out Room destination);
            if (isValidMove)
            {
                Location = destination;
            }

            return isValidMove;
        }
    }
}
