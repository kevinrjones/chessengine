using System;

namespace Game
{
    public class ZobristHashing
    {
        public Random random { get; private set; }

        public ZobristHashing()
        {
            random = new Random(101010);
        }
    }
}