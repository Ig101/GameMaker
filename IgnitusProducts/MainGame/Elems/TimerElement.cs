using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class TimerElement: HudElement
    {
        PressButtonAction action;
        float time;
        float startingTime;
        bool skippable;
        bool repeatable;
        bool expired;

        public bool Repeatable { get { return repeatable; } }
        public bool Expired { get { return expired; } }
        public bool Skippable { get { return skippable; } set { skippable = value; } }
        public PressButtonAction Action { get { return action; } }
        public float Time { get { return time; } set { time = value; } }
        public float StartingTime { get { return startingTime; } set { startingTime = value; } }

        public TimerElement(string name, PressButtonAction action, float startingTime, bool skippable, bool repeatable)
            :base(name,0,0,0,0,true,false,false)
        {
            this.skippable = skippable;
            this.startingTime = startingTime;
            this.action = action;
            this.time = startingTime;
            this.expired = false;
            this.repeatable = repeatable;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if (!expired)
            {
                time -= milliseconds / 1000;
                if (time <= 0 || (skippable && (state.KeysState[1] && !prevState.KeysState[1] ||
                    state.LeftButtonState && !prevState.LeftButtonState)))
                {
                    action(game, mode, this);
                    time = startingTime;
                    if (!repeatable) expired = true;
                }
            }
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {

        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {

        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "timer";
        }
    }
}
