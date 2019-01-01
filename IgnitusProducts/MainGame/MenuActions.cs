using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public static partial class MenuActions
    {
        public static void GoBack(IgnitusGame game, Mode mode, HudElement element)
        {
            game.GoToMode(mode.Parent);
        }
    }
}
