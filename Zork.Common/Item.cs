namespace Zork.Common
{
    public class Item
    {
        public string Name { get; }

        public string LookDescription { get; }

        public string InventoryDescription { get; }

        public bool IsValuable { get; }

        public Item(string name, string lookDescription, string inventoryDescription, bool isValuable)
        {
            Name = name;
            LookDescription = lookDescription;
            InventoryDescription = inventoryDescription;
            IsValuable = isValuable;
        }

        public override string ToString() => Name;
    }
}