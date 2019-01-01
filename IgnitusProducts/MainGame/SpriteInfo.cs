using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public struct PositionParametres
    {
        Vector2 translation;
        float rotation;
        float scale;

        public Vector2 Translation { get { return translation; } }
        public float Rotation { get { return rotation; } }
        public float Scale { get { return scale; } }

        public PositionParametres(Vector2 translation, float rotation, float scale)
        {
            this.translation = translation;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

    public class FrameInfo
    {
        PositionParametres presolvedParametres;
        PositionParametres tempParametres;
        Matrix wholeMatrix;
        bool hiden;

        public Matrix WholeMatrix { get { return wholeMatrix; } set { wholeMatrix = value; } }
        public PositionParametres PresolvedParametres { get { return presolvedParametres; } }
        public PositionParametres TempParametres { get { return tempParametres; } }
        public bool Hiden { get { return hiden; } }

        public FrameInfo(Vector2 tempTranslation, float tempRotation, float tempScale, bool hiden)
        {
            this.hiden = hiden;
            this.tempParametres = new PositionParametres(tempTranslation, tempRotation, tempScale);
        }

        public void SetPresolvedParametres(PositionParametres parametres)
        {
            this.presolvedParametres = parametres;
        }
    }

    public class AnimationInfo
    {
        FrameInfo[] frames;

        public FrameInfo[] Frames { get { return frames; } }

        public AnimationInfo(int numOfFrames)
        {
            this.frames = new FrameInfo[numOfFrames];
        }
    }

    public class AnimationObject
    {
        AnimationInfo[] animations;
        Vector2 nodeCenter;
        bool rotationImpact;
        bool startOfRotationImpact;
        bool colorisation;
        float depth;
        float backDepth;
        int parentObject;
        string textureName;

        public string TextureName { get { return textureName; } }
        public AnimationInfo[] Animations { get { return animations; } }
        public Vector2 NodeCenter { get { return nodeCenter; } }
        public bool RotationImpact { get { return rotationImpact; } }
        public bool StartOfRotationImpact { get { return startOfRotationImpact; } set { startOfRotationImpact = value; } }
        public bool Colorisation { get { return colorisation; } }
        public float Depth { get { return depth; } }
        public float BackDepth { get { return backDepth; } }
        public int ParentObject { get { return parentObject; } }

        public AnimationObject(string name, int numOfAnimations, Vector2 nodeCenter, bool rotationImpact, bool colorisation,
            float depth, float backDepth, int parentObject)
        {
            this.textureName = name;
            this.animations = new AnimationInfo[numOfAnimations];
            this.nodeCenter = nodeCenter;
            this.rotationImpact = rotationImpact;
            this.colorisation = colorisation;
            this.depth = depth;
            this.backDepth = backDepth;
            this.parentObject = parentObject;
        }
    }

    public class SpriteInfo
    {
        AnimationObject[] objects;
        int maxAnimation;
        int[] maxFrame;
        float[] frameSpeed;
        string name;

        public AnimationObject[] Objects { get { return objects; } }
        public string Name { get { return name; } }
        public int MaxAnimation { get { return maxAnimation; } }
        public int[] MaxFrame { get { return maxFrame; } }
        public float[] FrameSpeed { get { return frameSpeed; } }

        public SpriteInfo (string spriteName)
        {
            objects = null;
            maxAnimation = 0;
            maxFrame = new int[] { 0 };
            frameSpeed = new float[] { 1 };
            name = spriteName;
        }

        public SpriteInfo (string spriteName, string path)
        {
            objects = LoadAnimationFromFile(path);
            int[] frameNumbers;
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].RotationImpact)
                    {
                        if (i >= 0 && !objects[objects[i].ParentObject].RotationImpact)
                            objects[i].StartOfRotationImpact = true;
                    }
                }
                //
                frameNumbers = new int[objects[0].Animations.Length];
                for (int i = 0; i < frameNumbers.Length; i++)
                {
                    frameNumbers[i] = objects[0].Animations[i].Frames.Length - 1;
                }
            }
            else
            {
                frameNumbers = new int[] { 0 };
            }
            //target = new Sprite(spriteName,(byte)(frameNumbers.Length-1),frameNumbers, emitters);
            this.maxAnimation = frameNumbers.Length - 1;
            this.maxFrame = frameNumbers;
            this.frameSpeed = new float[maxFrame.Length];
            for (int i = 0; i < frameSpeed.Length;i++ )
            {
                frameSpeed[i] = 1f;
            }
            this.name = spriteName;
        }

        AnimationObject[] LoadAnimationFromFile(string path)
        {
            byte[] bytes = Magic.Restore(path);
            string megaMegaString = Encoding.UTF8.GetString(bytes);
            string[] mmS = megaMegaString.Split(new char[] { '#' });
            string megaString = mmS[0];
            AnimationObject[] objects;
            if (mmS[0] == "")
            {
                objects = null;
            }
            else
            {
                //Objects Info
                string[] subStrings = megaString.Split(new char[] { '\n' });
                string[] nodesConfig = subStrings[1].Split(new char[] { ';' });
                string[] centersConfig = subStrings[2].Split(new char[] { ';' });
                string[] colorisationConfig = subStrings[3].Split(new char[] { ';' });
                string[] depthConfig = subStrings[4].Split(new char[] { ';' });
                string[] rotationConfig = subStrings[5].Split(new char[] { ';' });
                objects = new AnimationObject[nodesConfig.Length];
                int numOfAnimas = subStrings.Length - 7;
                for (int i = 0; i < nodesConfig.Length; i++)
                {
                    string[] strNodes = nodesConfig[i].Split(new char[] { '*' });
                    string textureName = strNodes[0];
                    int parentMesh = int.Parse(strNodes[2]);
                    string[] centers = centersConfig[i].Split(new char[] { '*' });
                    Vector2 nodeCenter = new Vector2(float.Parse(centers[0]), float.Parse(centers[1]));
                    bool colorisation = colorisationConfig[i] != "0";
                    string[] depths = depthConfig[i].Split(new char[] { '*' });
                    float depth = int.Parse(depths[0]);
                    float backDepth = int.Parse(depths[1]);
                    bool rotationImpact = rotationConfig[i] != "0";
                    objects[i] = new AnimationObject(textureName, numOfAnimas, nodeCenter, rotationImpact,
                        colorisation, depth, backDepth, parentMesh);
                }
                for (int i = 7; i < subStrings.Length; i++)
                {
                    string[] massives = subStrings[i].Split(new char[] { '@' });
                    string[] frames = massives[1].Split(new char[] { ';' });
                    for (int o = 0; o < objects.Length; o++)
                    {
                        objects[o].Animations[i - 7] = new AnimationInfo(frames.Length);
                    }
                    for (int f = 0; f < frames.Length; f++)
                    {
                        string[] objectsNorm = frames[f].Split(new char[] { ':' });
                        for (int o = 0; o < objects.Length; o++)
                        {
                            string[] coords = objectsNorm[o + 1].Split(new char[] { '*' });
                            bool hiden = coords[0] != "0";
                            Vector2 translation = new Vector2(float.Parse(coords[1]), float.Parse(coords[2]));
                            float rotation = float.Parse(coords[3]);
                            float scale = float.Parse(coords[4]);
                            objects[o].Animations[i - 7].Frames[f] = new FrameInfo(translation, rotation, scale, hiden);
                        }
                        //Calculate presolvedStats
                        for (int o = 0; o < objects.Length; o++)
                        {
                            PositionParametres presolvedParametres = GetPresolvedParametres(i - 7, f, o, objects, 0, out Matrix matrix);
                            objects[o].Animations[i - 7].Frames[f].SetPresolvedParametres(presolvedParametres);
                            objects[o].Animations[i - 7].Frames[f].WholeMatrix = matrix;
                        }
                    }
                }
            }
            //PE
            /*if (mmS.Length > 1)
            {
                string[] emitterStrings = mmS[1].Split(new char[] { '\n' });
                emitters = new EmittersInfo[emitterStrings.Length];
                for (int i = 0; i < emitterStrings.Length; i++)
                {
                    string[] emitterProps = emitterStrings[i].Split(new char[] { ';' });
                    bool colorisation = emitterProps[0] == "1";
                    int attachedMesh = int.Parse(emitterProps[1]);
                    Color colorStart = new Color(byte.Parse(emitterProps[2]), byte.Parse(emitterProps[3]), byte.Parse(emitterProps[4]));
                    int numPerSecond = int.Parse(emitterProps[5]);
                    Vector3 positionStart = new Vector3(float.Parse(emitterProps[6]) * 0.7f, float.Parse(emitterProps[6]) * 0.7f,
                        -float.Parse(emitterProps[7]));
                    Vector3 positionRange = new Vector3(float.Parse(emitterProps[8]), float.Parse(emitterProps[9]), float.Parse(emitterProps[10]));
                    string textureName = emitterProps[11];
                    float velocityStart = float.Parse(emitterProps[12]);
                    float sizeStart = float.Parse(emitterProps[13]);
                    float alphaStart = float.Parse(emitterProps[14]);
                    float lifeTimeStart = float.Parse(emitterProps[15]);
                    float angleStart = float.Parse(emitterProps[16]);
                    float velocityRange = float.Parse(emitterProps[17]);
                    float sizeRange = float.Parse(emitterProps[18]);
                    float angleRange = float.Parse(emitterProps[19]);
                    float velocitySpeed = float.Parse(emitterProps[20]);
                    float angleSpeed = float.Parse(emitterProps[21]);
                    float sizeSpeed = float.Parse(emitterProps[22]);
                    float alphaSpeed = float.Parse(emitterProps[23]);
                    bool[][] hiden;
                    if (objects == null)
                    {
                        hiden = new bool[1][];
                        hiden[0] = new bool[1];
                    }
                    else
                    {
                        hiden = new bool[objects[0].Animations.Length][];
                        for (int j = 0; j < hiden.Length; j++)
                        {
                            hiden[j] = new bool[objects[0].Animations[j].Frames.Length];
                        }
                        if (emitterProps.Length > 24)
                        {
                            for (int x = 0; x < objects[0].Animations.Length; x++)
                            {
                                string[] animas = emitterProps[x + 24].Split(new char[] { ':' });
                                for (int y = 0; y < objects[0].Animations[x].Frames.Length; y++)
                                {
                                    hiden[x][y] = animas[y] == "1";
                                }
                            }
                        }
                    }
                    emitters[i] = new EmittersInfo(normalSpawning, normalRule, textureName, positionStart, velocityStart / 2,
                        sizeStart * 3f, alphaStart, lifeTimeStart, angleStart, positionRange, velocityRange / 2, sizeRange * 3f, angleRange, velocitySpeed / 2,
                        angleSpeed, sizeSpeed * 3f, alphaSpeed, numPerSecond, colorStart, attachedMesh, colorisation, hiden);
                }
            }
            else
            {
                emitters = null;
            }*/
            return objects;
        }

        public PositionParametres GetPresolvedParametres(int animationNumber, int frameNumber, int objectNumber,
            AnimationObject[] objects, float rotation, out Matrix matrix)
        {
            matrix = GetTranslationMatrix(animationNumber, frameNumber, objectNumber, objects, rotation);
            return new PositionParametres(GetTempPosition(matrix), GetTempRotation(matrix), GetTempScale(matrix));
        }

        Vector2 GetTempPosition(Matrix matrix)
        {
            Vector2 pos = new Vector2(0, 0);
            Vector2 nextPosition = Vector2.Transform(pos, matrix);
            return nextPosition;
        }

        float GetTempRotation(Matrix matrix)
        {
            Vector2 translation = Vector2.Transform(Vector2.Zero, matrix);
            Vector2 resultVector = Vector2.Transform(new Vector2(1, 0), matrix) - translation;
            return (float)Math.Atan2(resultVector.Y, resultVector.X);
        }

        float GetTempScale(Matrix matrix)
        {
            Vector2 translation = Vector2.Transform(Vector2.Zero, matrix);
            Vector2 resultVector = Vector2.Transform(new Vector2(1, 0), matrix) - translation;
            return (float)Math.Sqrt(resultVector.X * resultVector.X + resultVector.Y * resultVector.Y);
        }

        Matrix GetTranslationMatrix(int animationNumber, int frameNumber, int objectNumber, AnimationObject[] objects, float rotation)
        {
            AnimationObject obj = objects[objectNumber];
            FrameInfo info = obj.Animations[animationNumber].Frames[frameNumber];
            Matrix returnedMatrix = Matrix.CreateTranslation(-obj.NodeCenter.X, -obj.NodeCenter.Y, 0) *
                    Matrix.CreateScale(info.TempParametres.Scale, info.TempParametres.Scale, 0) *
                    Matrix.CreateRotationZ(info.TempParametres.Rotation + (obj.StartOfRotationImpact ? rotation : 0)) *
                    Matrix.CreateTranslation(obj.NodeCenter.X, obj.NodeCenter.Y, 0) *
                    Matrix.CreateTranslation(info.TempParametres.Translation.X, info.TempParametres.Translation.Y, 0);
            if (obj.ParentObject >= 0)
            {
                return returnedMatrix * GetTranslationMatrix(animationNumber, frameNumber, obj.ParentObject, objects, rotation);
            }
            else
            {
                return returnedMatrix;
            }
        }
    }
}
