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

        private static bool Move(Commands command)
        {
            Assert.IsTrue(IsDirection(command), "Invalid direction");

            bool didMove = false;

            switch (command)
            {
                case Commands.North when _location.Row < _world.Rooms.GetLength(0) - 1:
                    _location.Row++;
                    didMove = true;
                    break;

                case Commands.South when _location.Row > 0:
                    _location.Row--;
                    didMove = true;
                    break;

                case Commands.East when _location.Column < _world.Rooms.GetLength(1) - 1:
                    _location.Column++;
                    didMove = true;
                    break;

                case Commands.West when _location.Column > 0:
                    _location.Column--;
                    didMove = true;
                    break;
            }

            return didMove;
        }

        private static (int Row, int Column) _location = (1, 1);

    }
}
