using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
   public class AnyKeyElement:HudElement
    {

        PressButtonAction action;

        public AnyKeyElement (string name, PressButtonAction action)
            : base(name,0,0,0,0,true,false,false)
        {
            this.action = action;
        }

        public override void Draw(IgnitusGame game, Microsoft.Xna.Framework.Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            bool b = state.LeftButtonState && !prevState.LeftButtonState ||
               state.RightButtonState && !prevState.RightButtonState;
            if(!b)
            {
                for(int i = 0; i<state.KeysState.Length;i++)
                {
                    b = state.KeysState[i] && !prevState.KeysState[i];
                    if (b) break;
                }
            }
            if (b) action(game, mode, this);
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        public override string ToString()
        {
            return "AnyKeyElement";
        }
    }
}
