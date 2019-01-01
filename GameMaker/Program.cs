using System;
using Game;

namespace GameMaker
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1Shell(Consts.loadingTime, Consts.texturesOnStart, Consts.loadingScreenElements, Consts.mainElements,
                Consts.profileSavePath, Consts.SaveProfile, Consts.LoadProfile,new GameElement(Consts.fonName)))
                game.Run();
        }
    }
#endif
}
