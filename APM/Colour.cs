using System;

namespace Two10.APM
{
    class Colour : IDisposable
    {
        private ConsoleColor oldColour;
        public Colour(ConsoleColor newColour)
        {
            this.oldColour = Console.ForegroundColor;
            Console.ForegroundColor = newColour;
        }

        public void Dispose()
        {
            Console.ForegroundColor = oldColour;
        }

    }
}
