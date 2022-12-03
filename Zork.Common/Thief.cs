using System;
using System.Collections.Generic;
namespace Zork.Common
{
    public class Thief
    {
        public Room CurrentRoom
        {
            get => _currentRoom;
            set
            {
                if (_currentRoom != value)
                {
                    _currentRoom = value;
                }
            }
        }

        public bool IsDead;

        public IEnumerable<Item> Inventory => _inventory;
        public Thief(World world)
        {
            _world = world;
            ChangeRoom();

            _inventory = new List<Item>();
        }

        public void ChangeRoom()
        {
            Random rndRoom = new Random();
            int randomRoomID = rndRoom.Next(_world.Rooms.Length);
            CurrentRoom = _world.Rooms[randomRoomID];

            Random rndPercentage = new Random();
            int randomNum = rndPercentage.Next(101);
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
        private Room _currentRoom;
        private readonly List<Item> _inventory;
    }
}
