using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public abstract class ListElementItem
    {
        public abstract void Draw(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor, int width, int height, int itemPosition, int verShift,
            bool selected, bool pressed);
    }

    public class ListElement : HudElement
    {
        public delegate void ListItemClick (IgnitusGame game, Mode mode, ListElement element, ListElementItem item);
        public delegate void PassiveAction(IgnitusGame game, Mode mode, ListElement element);

        int? linkedSlideElement;
        float selfPosition;
        List<ListElementItem> items = new List<ListElementItem>();
        ListItemClick action;
        PassiveAction passive;
        int elementHeight;
        RenderTarget2D screen;

        int selectedElement;
        int pressedElement;

        public int? LinkedSlideElement { get { return linkedSlideElement; } }
        public float SelfPosition { get { return selfPosition; } set { selfPosition = value>1?1:value<0?0:value; } }
        public List<ListElementItem> Items { get { return items; } }
        public ListItemClick Action { get { return action; } }
        public PassiveAction Passive { get { return passive; } }
        public int ElementHeight { get { return elementHeight; } }


        public ListElement (string name, int x, int y, int width, int height, ListItemClick action, PassiveAction passive, int elementHeight,
            bool ignoreAnimation, bool ignoreBackAnimation):
            base (name,x,y,width,height, false, ignoreAnimation, ignoreBackAnimation)
        {

        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if(linkedSlideElement!=null && mode.Elements[(int)linkedSlideElement].ToString()=="SlideElement")
            {
                this.selfPosition = ((SlideElement)mode.Elements[(int)linkedSlideElement]).Position;
            }
            passive(game, mode, this);
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            //TODO Update
            /*if(pressedElement>=0)
            {
                if((way && !state.LeftButtonState && prevState.LeftButtonState) ||
                    (!way && !state.KeysState[0] && prevState.KeysState[0]))
                {
                    int xShift = pressedElement % elementsPerLine;
                    int yShift = pressedElement / elementsPerLine;
                    if (way && correctedMousePos.X >= xShift * iconsSide && correctedMousePos.X <= (xShift + 1) * iconsSide &&
                            correctedMousePos.Y >= yShift * iconsSide && correctedMousePos.Y <= (yShift + 1) * iconsSide)
                    {
                        pressAction(game, mode, this, pressedElement);
                    }
                    pressedElement = -1;
                    //selectedElement = -1;
                }
            }
            else
            {
                if (mode.MouseUse > 0)
                {
                    selectedElement = -1;
                    for (int i = 0; i < Math.Min(elements.Count, wholeNumber); i++)
                    {
                        int xShift = i % elementsPerLine;
                        int yShift = i / elementsPerLine;
                        if (correctedMousePos.X >= xShift * iconsSide && correctedMousePos.X <= (xShift + 1) * iconsSide &&
                            correctedMousePos.Y >= yShift * iconsSide && correctedMousePos.Y <= (yShift + 1) * iconsSide)
                        {
                            selectedElement = i;
                        }
                    }
                }
                if ((way = state.LeftButtonState && !prevState.LeftButtonState) ||
                    (mode.KeyboardUse && state.KeysState[0] && !prevState.KeysState[0]))
                {
                    pressedElement = selectedElement;
                }
            }
            if(mode.MouseUse<=0 && mode.KeyboardUse)
            {
                if (!state.KeysState[5] && !state.KeysState[4] && !state.KeysState[3] && !state.KeysState[2])
                {
                    mode.ZeroStepInterval();
                }
                else if (state.KeysState[5] && mode.StepInterval <= 0)
                {
                    selectedElement += selectedElement>=0?elementsPerLine:0;
                    mode.SetStepInterval();
                    if (selectedElement >= wholeNumber || selectedElement >= elements.Count || selectedElement<0)
                    {
                        selectedElement = -1;
                        mode.StepBetweenElements(true, false);
                    }
                }
                else if (state.KeysState[4] && mode.StepInterval <= 0)
                {
                    selectedElement -= elementsPerLine;
                    mode.SetStepInterval();
                    if (selectedElement < 0)
                    {
                        selectedElement = -1;
                        mode.StepBetweenElements(false, false);
                    }
                }
                else if (state.KeysState[2] && mode.StepInterval <= 0)
                {
                    mode.SetStepInterval();
                    int line = selectedElement / elementsPerLine;
                    if (selectedElement == 0) selectedElement = Math.Min(elements.Count,elementsPerLine) - 1;
                    else
                    {
                        selectedElement--;
                        int secondLine = selectedElement / elementsPerLine;
                        if (secondLine < line) selectedElement += elementsPerLine;
                    }
                }
                else if (state.KeysState[3] && mode.StepInterval <= 0)
                {
                    int line = selectedElement / elementsPerLine;
                    mode.SetStepInterval();
                    selectedElement++;
                    int secondLine = selectedElement / elementsPerLine;
                    if (selectedElement >= elements.Count) selectedElement = line * elementsPerLine;
                    else if (secondLine > line) selectedElement -= elementsPerLine;
                }
            }*/
        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            if (screen != null)
            {
                game.DrawSprite(screen, new Rectangle(X + Width / 2, Y + Height / 2, Width, Height), Color.White, 0, SpriteEffects.None, 0);
            }
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            if(screen!=null)
            {
                screen.Dispose();
            }
            screen = new RenderTarget2D(game.GraphicsDevice, Width, Height);
            game.GraphicsDevice.SetRenderTarget(screen);
            game.GraphicsDevice.Clear(Color.Black);
            game.BeginDrawingNoScale(SpriteSortMode.FrontToBack, null);
            int height = items.Count * elementHeight;
            int shift = -(int)(height * selfPosition);
            for (int i = 0; i < items.Count;i++ )
            {
                int position = shift + i * ElementHeight;
                if (position > -ElementHeight - 20 && position < Height + ElementHeight + 20)
                {
                    items[i].Draw(game, fonColor, Width, ElementHeight, i, position, i==selectedElement, i==pressedElement);
                }
            }
            game.EndDrawing();
            game.GraphicsDevice.SetRenderTarget(null);
        }

        public override string ToString()
        {
            return "ListElement";
        }
    }
}
