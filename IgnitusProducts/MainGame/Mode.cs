using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public struct MatrixColorCombo
    {
        Color color;
        Matrix matrix;

        public Color Color { get { return color; } }
        public Matrix Matrix { get { return matrix; } }

        public MatrixColorCombo (Matrix matrix, Color color)
        {
            this.matrix = matrix;
            this.color = color;
        }
    }

    public class Mode
    {
        public delegate MatrixColorCombo GetAnimationTransformation(IgnitusGame game, float animationProgress, Color fonColor);
        public delegate void ActionOnOpening(IgnitusGame game, Mode mode);

        ActionOnOpening action;
        GetAnimationTransformation animationMatrix;
        Mode parent;
        HudElement[] elements;
        int tempElement;
        string name;

        float animationProgress;
        float animationSpeed;
        bool animationDirection;

        bool cursorEnabled;
        float mouseUse;

        bool keyboardUse;
        float stepInterval;

        public ActionOnOpening Action { get { return action; } }
        public GetAnimationTransformation AnimationMatrix { get { return animationMatrix; } }
        public bool KeyboardUse { get { return keyboardUse; } }
        public float StepInterval { get { return stepInterval; } }
        public float MouseUse { get { return mouseUse; } }
        public int TempElement { get { return tempElement; } set { tempElement = value; } }
        public string Name { get { return name; } }
        public bool CursorEnabled { get { return cursorEnabled; } set{cursorEnabled=value;}}
        public Mode Parent { get { return parent; } }
        public HudElement[] Elements { get { return elements; } }
        public float AnimationProgress { get { return animationProgress; } }
        public float AniationSpeed { get { return animationSpeed; } }
        public bool AnimationDirection { get { return animationDirection; } set { 
            animationDirection = value; 
            if(animationDirection)
            {
                animationProgress = 0.0001f;
            }
            else
            {
                animationProgress = 0.9999f;
            }
        } }

        public Mode (Mode parent, HudElement[] elements, float animationSpeed, string name, GetAnimationTransformation matrix,
            ActionOnOpening action, bool keyboardUse)
        {
            this.action = action;
            this.keyboardUse = keyboardUse;
            this.animationMatrix = matrix;
            this.parent = parent;
            this.elements = elements;
            this.name = name;
            this.AnimationDirection = true;
            this.cursorEnabled = true;
            this.animationSpeed = animationSpeed;
            while(elements[tempElement].Ignored && tempElement<elements.Length-1)
            {
                tempElement++;
            }
            this.animationProgress = 1;
        }

        public void ZeroStepInterval ()
        {
            this.stepInterval = 0;
        }

        public void SetStepInterval()
        {
            this.stepInterval = 0.2f;
        }

        public void StepBetweenElements (bool direction, bool through)
        {
            int element = tempElement;
            do
            {
                element+= direction?1:-1;
                if (element >= elements.Length) element = 0;
                if (element < 0) element = elements.Length - 1;
            }
            while (element != tempElement && elements[element].Ignored);
            tempElement = element;
            if(!through) this.stepInterval = 0.2f;
        }

        public void ModeUpdate(float milliseconds)
        {
            milliseconds /= 1000;
            this.animationProgress += animationSpeed * milliseconds * (animationDirection ? 1 : -1);
            if (this.animationProgress > 1f) this.animationProgress = 1f;
            if (this.animationProgress < 0f) this.animationProgress = 0f;
            mouseUse -= milliseconds;
            if (mouseUse < 0) mouseUse = 0;
            stepInterval -= milliseconds;
            if (stepInterval < 0) stepInterval = 0;
        }

        public void Update(IgnitusGame game, ControlsState tempState, ControlsState prevState, float milliseconds)
        {           
            bool updated = false;
            if (tempState.MousePosition != prevState.MousePosition ||
                tempState.LeftButtonState != prevState.LeftButtonState ||
                tempState.MiddleButtonState != prevState.MiddleButtonState ||
                tempState.RightButtonState != prevState.RightButtonState ||
                tempState.WheelState != prevState.WheelState) mouseUse = 0.2f;
            if (mouseUse > 0 && !tempState.LeftButtonState && !tempState.KeysState[0])
            {
                for (int i = elements.Length - 1; i >= 0; i--)
                {
                    elements[i].PassiveUpdate(game, this, tempState, prevState, milliseconds);
                    if (!updated && !elements[i].Ignored && elements[i].Visible && tempState.MousePosition.X >= elements[i].X-20 &&
                        tempState.MousePosition.Y >= elements[i].Y-20 &&
                        tempState.MousePosition.X <= elements[i].X + elements[i].Width+20 &&
                        tempState.MousePosition.Y <= elements[i].Y + elements[i].Height+20)
                    {
                        elements[i].Update(game, this, tempState, prevState, milliseconds);
                        tempElement = i;
                        updated = true;
                    }
                }
                if (!updated) 
                {
                    if (!keyboardUse)
                    {
                        tempElement = 0;
                        while (elements[tempElement].Ignored && tempElement < elements.Length - 1)
                        {
                            tempElement++;
                        }
                    }
                    if (keyboardUse) 
                        elements[tempElement].Update(game, this, tempState, prevState, milliseconds);
                }
            }
            else
            {
                for (int i = elements.Length - 1; i >= 0; i--)
                {
                    elements[i].PassiveUpdate(game, this, tempState, prevState,milliseconds);
                }
                //
                if (keyboardUse || //(tempState.LeftButtonState) ||
                    tempState.MousePosition.X >= elements[tempElement].X - 20 &&
                    tempState.MousePosition.Y >= elements[tempElement].Y - 20 &&
                    tempState.MousePosition.X <= elements[tempElement].X + elements[tempElement].Width + 20 &&
                    tempState.MousePosition.Y <= elements[tempElement].Y + elements[tempElement].Height + 20)
                {
                    elements[tempElement].Update(game, this, tempState, prevState, milliseconds);
                }
            }
        }

        public void Draw(IgnitusGame game, bool mainMode, Color fonColor, bool noAnimation, float milliseconds)
        {
            float animationProgress = mainMode? this.animationProgress : noAnimation?1:game.GetTempMode().animationProgress;
            MatrixColorCombo matrixColor = animationMatrix(game, animationProgress, fonColor);
            bool changeMatrix;
            if(matrixColor.Matrix == new Matrix())
            {
                game.BeginDrawing(SpriteSortMode.Immediate,null);
                changeMatrix = false;
            }
            else
            {
                game.BeginDrawing(SpriteSortMode.Immediate, null, matrixColor.Matrix);
                changeMatrix = true;
            }
            for(int i = 0; i<elements.Length;i++)
            {
                if (elements[i].Visible)
                {
                    bool animationShow = ((elements[i].IgnoreAnimation && animationDirection) && noAnimation ||
                        (elements[i].IgnoreBackAnimation && !animationDirection));
                    if (animationShow && changeMatrix)
                    {
                        game.EndDrawing();
                        game.BeginDrawing(SpriteSortMode.Immediate, null);
                    }
                    elements[i].Draw(game, matrixColor.Matrix, !animationShow?matrixColor.Color:Color.White, milliseconds);
                    if (animationShow && changeMatrix)
                    {
                        game.EndDrawing();
                        game.BeginDrawing(SpriteSortMode.Immediate, null, matrixColor.Matrix);
                    }
                }
            }
            game.EndDrawing();
        }

        public void DrawPreActionsUpdate (IgnitusGame game, Color fonColor)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].DrawPreActionsUpdate(game, fonColor);
            }
        }

        //
        public static MatrixColorCombo Nothing(IgnitusGame game, float animationProgress, Color color)
        {
            return new MatrixColorCombo(new Matrix(), color);
        }

        public static MatrixColorCombo BlackGlow(IgnitusGame game, float animationProgress, Color color)
        {
            return new MatrixColorCombo(new Matrix(), new Color((byte)(color.R * animationProgress),
                (byte)(color.G * animationProgress), (byte)(color.B * animationProgress)));
        }

        public static MatrixColorCombo AlphaGlow(IgnitusGame game, float animationProgress, Color color)
        {
            return new MatrixColorCombo(new Matrix(), new Color((byte)(color.R),
                (byte)(color.G), (byte)(color.B), (byte)(color.A*animationProgress)));
        }
        //
    }
}
