using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class ContextMenuElement : HudElement
    {
        string parentMode;

        public ContextMenuElement (string parentMode)
            :base ("context_menu",0,0,0,0,true,false,false)
        {
            this.parentMode = parentMode;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if(!state.KeysState[1] && prevState.KeysState[1])
            {
                game.GoToMode(parentMode);
            }
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        public override void Draw(IgnitusGame game, Microsoft.Xna.Framework.Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "ContextMenuElement";
        }
    }
}
