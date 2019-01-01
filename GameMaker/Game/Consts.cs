using GameMaker;
using Ignitus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class Consts
    {
        //Time before moving to the main menu at the start of application
        public static int loadingTime = 0;

        //Name of texture that used for the base menu fon
        public static string fonName = "";

        //All textures that should be loaded before loading (for loading screen)
        public static string[] texturesOnStart = new string[]
        {

        };

        //All extra loading screen elements (labels, sprites, etc.)
        public static HudElement[] loadingScreenElements = new HudElement[]
        {

        };

        //All extra main menu elements
        public static HudElement[] mainElements = new HudElement[]
        {

        };

        //Path of the profile directory (file profile.mrc will be held here)
        public static string profileSavePath = Environment.CurrentDirectory;

        //Method that brings profile information and analyse it
        public static void LoadProfile(GameElementShell gameElementShell, string profile)
        {
            GameElement gameElement = (GameElement)gameElementShell;
            //Put your logic here
        }

        //Method that creates profile information string
        public static string SaveProfile(GameElementShell gameElementShell)
        {
            GameElement gameElement = (GameElement)gameElementShell;
            //Put your logic here
            return null;
        }
    }
}
