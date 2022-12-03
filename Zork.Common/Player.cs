using System;
using System.Collections.Generic;

namespace Zork.Common
{
    public class Player
    {
        public event EventHandler<Room> LocationChange;
        public event EventHandler<int> MovesChanged;
        public event EventHandler<int> ScoreChanged;

        public Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                if (_currentRoom != value)
                {
                    _currentRoom = value;
                    LocationChange?.Invoke(this, _currentRoom);
                }
            }
        }

        public int Moves
        {
            get
            {
                return _moves;
            }
            set
            {
                if (_moves != value)
                {
                    _moves = value;
                    MovesChanged?.Invoke(this, _moves);
                }
            }
        }

        public int Score
        {
            get 
            { 
                return _score; 
            }
            set
            {
                if(_score != value)
                {
                    _score = value;
                    ScoreChanged?.Invoke(this, _score);
                }
            }
        }

        public int MaxHealth
        {
            get
            {
                return _maxHealth;
            }
            private set
            {
                if (_maxHealth != value)
                {
                    _maxHealth = value;
                }
            }

        }
        public int CurrentHealth
        {
            get
            {
                return _currentHealth;
            }
            set
            {              
                if(_currentHealth != value)
                {
                    _currentHealth = value;
                }
            }
    
        }

        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                if (_currentHealth <= 0)
                {
                    _isDead = true;
                }
            }
        }
        public IEnumerable<Item> Inventory => _inventory;

        public Player(World world, string startingLocation, string startingWeapon, int maxHealth)
        {
            _world = world;

            if (_world.RoomsByName.TryGetValue(startingLocation, out _currentRoom) == false)
            {
                throw new Exception($"Invalid starting location: {startingLocation}");
            }

            _inventory = new List<Item>();

            if(_world.ItemsByName.TryGetValue(startingWeapon, out _startingWeapon) == false)
            {
                throw new Exception($"Invalid starting weapon: {startingWeapon}");
            }
            _inventory.Add(_startingWeapon);

            _maxHealth = maxHealth;
            _currentHealth = maxHealth;

        }

        public bool Move(Directions direction)
        {
            bool didMove = _currentRoom.Neighbors.TryGetValue(direction, out Room neighbor);
            if (didMove)
            {
                CurrentRoom = neighbor;
            }

            return didMove;
        }

        public void AddItemToInventory(Item itemToAdd)
        {
            if (_inventory.Contains(itemToAdd))
            {
                throw new Exception($"Item {itemToAdd} already exists in inventory.");
            }

            _inventory.Add(itemToAdd);
        }

        public void RemoveItemFromInventory(Item itemToRemove)
        {
            if (_inventory.Remove(itemToRemove) == false)
            {
                throw new Exception("Could not remove item from inventory.");
            }
        }

        private readonly World _world;
        private Item _startingWeapon;
        private Room _currentRoom;
        private int _moves;
        private int _score;

        private int _currentHealth;
        private int _maxHealth;
        private bool _isDead = false;

        private readonly List<Item> _inventory;
    }
}
