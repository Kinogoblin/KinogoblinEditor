namespace Kinogoblin.Playables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Playables;

    [Serializable]
    public class MaterialsBehavior : PlayableBehaviour
    {
        public bool changeDuringClip = true;
        public List<MaterialPropertis> materialPropertis = new List<MaterialPropertis>();
        public bool textureAnimationEnable = false;
        public List<TextureAnimation> textureAnimations = new List<TextureAnimation>();

        private Material material;

        private List<MaterialPropertis> materialPropertisDefault = new List<MaterialPropertis>();
        private List<TextureAnimation> textureAnimationsDefault = new List<TextureAnimation>();

        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            material = playerData as Material;

            if (material == null)
                return;

            if (!firstFrameHappened)
            {
                SaveDefaultValues();

                firstFrameHappened = true;
            }

            ChangeMaterialProperties(playable);

            if (textureAnimationEnable)
            {
                TextureAnimation(playable);
            }

        }

        public void SaveDefaultValues()
        {
            materialPropertisDefault = new List<MaterialPropertis>();
            foreach (var item in materialPropertis)
            {
                MaterialPropertis tempProp = new MaterialPropertis();
                tempProp = item;
                materialPropertisDefault.Add(tempProp);
            }
            foreach (var item in materialPropertisDefault)
            {
                switch (item.paramType)
                {
                    case MatPropType.ColorT:
                        item.colorProp = material.GetColor(item.propertyName);
                        break;
                    case MatPropType.TextureT:
                        item.textureProp = material.GetTexture(item.propertyName);
                        break;
                    case MatPropType.floatT:
                        item.floatProp = material.GetFloat(item.propertyName);
                        break;
                    case MatPropType.intT:
                        item.intProp = material.GetInt(item.propertyName);
                        break;
                    default:
                        break;
                }
            }
            if (textureAnimationEnable)
            {
                textureAnimationsDefault = new List<TextureAnimation>();
                foreach (var item in textureAnimations)
                {
                    TextureAnimation tempTextureAnim = new TextureAnimation();
                    tempTextureAnim = item;
                    textureAnimationsDefault.Add(tempTextureAnim);
                }
                foreach (var item in textureAnimationsDefault)
                {
                    item.texturePack.Add(material.GetTexture(item.propertyName));
                }
            }

        }

        public void ChangeMaterialProperties(Playable playable)
        {
            float totalWeight = 0;

            totalWeight = (float)playable.GetTime() / (float)playable.GetDuration();


            float remainmingWeight = 1 - totalWeight;

            if (changeDuringClip)
            {
                foreach (var item in materialPropertis)
                {
                    switch (item.paramType)
                    {
                        case MatPropType.ColorT:
                            material.SetColor(item.propertyName, (item.colorProp * remainmingWeight + item.colorPropFinish * totalWeight));
                            break;
                        case MatPropType.TextureT:
                            if (remainmingWeight > 0.5f)
                            {
                                material.SetTexture(item.propertyName, (item.textureProp));
                            }
                            else
                            {
                                material.SetTexture(item.propertyName, (item.texturePropFinish));
                            }
                            break;
                        case MatPropType.floatT:
                            material.SetFloat(item.propertyName, (item.floatProp * remainmingWeight + item.floatPropFinish * totalWeight));
                            break;
                        case MatPropType.intT:
                            material.SetInt(item.propertyName, (int)(item.intProp * remainmingWeight + item.intPropFinish * totalWeight));
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                foreach (var item in materialPropertis)
                {
                    switch (item.paramType)
                    {
                        case MatPropType.ColorT:
                            material.SetColor(item.propertyName, item.colorProp);
                            break;
                        case MatPropType.TextureT:
                            material.SetTexture(item.propertyName, item.textureProp);
                            break;
                        case MatPropType.floatT:
                            material.SetFloat(item.propertyName, item.floatProp);
                            break;
                        case MatPropType.intT:
                            material.SetInt(item.propertyName, item.intProp);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void TextureAnimation(Playable playable)
        {

            float totalWeight = 0;

            totalWeight = (float)playable.GetTime() / (float)playable.GetDuration();

            foreach (var item in textureAnimations)
            {
                var frameID = (int)(item.texturePack.Count * totalWeight);
                var frame = item.texturePack[frameID];
                if (frame != null)
                    material.SetTexture(item.propertyName, frame);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            firstFrameHappened = false;

            if (material == null)
                return;

            foreach (var item in materialPropertisDefault)
            {
                switch (item.paramType)
                {
                    case MatPropType.ColorT:
                        material.SetColor(item.propertyName, item.colorProp);
                        break;
                    case MatPropType.TextureT:
                        material.SetTexture(item.propertyName, item.textureProp);
                        break;
                    case MatPropType.floatT:
                        material.SetFloat(item.propertyName, item.floatProp);
                        break;
                    case MatPropType.intT:
                        material.SetInt(item.propertyName, item.intProp);
                        break;
                    default:
                        break;
                }
            }

            if (textureAnimationEnable)
            {
                foreach (var item in textureAnimationsDefault)
                {
                    material.SetTexture(item.propertyName, item.texturePack[0]);
                }
            }

            base.OnBehaviourPause(playable, info);
        }

    }


    [Serializable]
    public class MaterialPropertis
    {
        public string propertyName = "";
        public MatPropType paramType;
        [Header("Start value")]
        public Color colorProp = Color.white;
        public Texture textureProp;
        public float floatProp;
        public int intProp;

        [Header("Finish value")]

        public Color colorPropFinish = Color.black;
        public Texture texturePropFinish;
        public float floatPropFinish;
        public int intPropFinish;
    }

    [Serializable]
    public class TextureAnimation
    {
        public string propertyName = "";
        public List<Texture> texturePack = new List<Texture>();
    }

    public enum MatPropType
    {
        None,
        ColorT,
        TextureT,
        floatT,
        intT
    }
}