using System.Data;

namespace Zork
{
    internal class Player
    {

        public Room CurrentRoom 
        {
            get
            {
                return _world.Rooms[_location.Row, _location.Column];
            }
        }

        public int Score { get; }

        public int Moves { get; }

        public Player(World world)
        {
            _world = world;
        }

        private World _world;
        private static (int Row, int Column) _location = (1, 1);

    }
}
