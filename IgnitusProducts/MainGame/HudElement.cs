using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public abstract class HudElement
    {
        int x;
        int y;
        int width;
        int height;
        bool ignored;
        string name;
        bool ignoreAnimation;
        bool ignoreBackAnimation;
        bool visible;

        public bool IgnoreBackAnimation { get { return ignoreBackAnimation; } }
        public bool Visible { get { return visible; } set { visible = value; } }
        public bool IgnoreAnimation { get { return ignoreAnimation; } }
        public string Name { get { return name; } set { name = value; } }
        public bool Ignored { get { return ignored || !visible; } set { ignored = value; } }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }

        protected HudElement (string name, int x, int y, int width, int height, bool ignored, 
            bool ignoreAnimation, bool ignoreBackAnimation)
        {
            this.ignoreBackAnimation = ignoreBackAnimation;
            this.visible = true;
            this.ignoreAnimation = ignoreAnimation;
            this.name = name;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.ignored = ignored;
        }

        public abstract void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds);

        public abstract void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds);

        public abstract void Draw(IgnitusGame game, Matrix animation, Color fonColor, float milliseconds);

        public abstract void DrawPreActionsUpdate(IgnitusGame game, Color fonColor);

        public virtual void SetElement (int variable)
        {

        }

        public Point TransformPointToElementCoords (Point point)
        {
            return new Point(point.X - x, point.Y - y);
        }

        public bool CheckMousePositionInElement (Point position)
        {
            position = TransformPointToElementCoords(position);
            return position.X >= -20 && position.Y >= -20 && position.X <= width+20 && position.Y <= height+20;
        }

        public void ArrowsMechanics (Mode mode, ControlsState state)
        {
            if (mode.KeyboardUse)
            {
                if(!state.KeysState[5] && !state.KeysState[4])
                {
                    mode.ZeroStepInterval();
                }
                else if (state.KeysState[5] && mode.StepInterval <= 0)
                {
                    mode.StepBetweenElements(true, false);
                    mode.Elements[mode.TempElement].SetElement(0);
                }
                else if(state.KeysState[4] && mode.StepInterval<=0)
                {
                    mode.StepBetweenElements(false, false);
                    mode.Elements[mode.TempElement].SetElement(1000);
                }
            }
        }

        public void ArrowsMechanicsIgnore (Mode mode, ControlsState state)
        {
            if(mode.KeyboardUse && !CheckMousePositionInElement(state.MousePosition))
            {
                if (state.KeysState[4])
                {
                    mode.StepBetweenElements(false, true);
                }
                else
                {
                    mode.StepBetweenElements(true, true);
                }
            }
        }
    }
}
