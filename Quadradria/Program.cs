using System;

namespace Quadradria
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Quadradria())
                game.Run();
        }
    }
}
