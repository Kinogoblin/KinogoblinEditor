// namespace Kinogoblin.Playables
// {
//     using System.Collections;
//     using System.Collections.Generic;
//     using UnityEngine;
//     using UnityEngine.Playables;

//     public class MaterialsMixer : PlayableBehaviour
//     {
//         public Material material;

//         private bool firstFrameHappened;
//         private List<MaterialPropertis> materialPropertisDefault = new List<MaterialPropertis>();
//         private List<MaterialPropertis> materialPropertisTemp = new List<MaterialPropertis>();

//         public override void ProcessFrame(Playable playable, FrameData info, object playerData)
//         {
//             material = playerData as Material;

//             if (material == null)
//                 return;

//             if (!firstFrameHappened)
//             {
//                 foreach (var item in materialPropertisDefault)
//                 {
//                     switch (item.paramType)
//                     {
//                         case MatPropType.ColorT:
//                             item.colorProp = material.GetColor(item.propertyName);
//                             break;
//                         case MatPropType.TextureT:
//                             item.textureProp = material.GetTexture(item.propertyName);
//                             break;
//                         case MatPropType.floatT:
//                             item.floatProp = material.GetFloat(item.propertyName);
//                             break;
//                         case MatPropType.intT:
//                             item.intProp = material.GetInt(item.propertyName);
//                             break;
//                         default:
//                             break;
//                     }
//                 }
//                 firstFrameHappened = true;
//             }

//             int inputCount = playable.GetInputCount();

//             float totalWeight = 0;

//             for (int i = 0; i < inputCount; i++)
//             {
//                 float inputWeight = playable.GetInputWeight(i);
//                 ScriptPlayable<MaterialsBehavior> inputPlayable = (ScriptPlayable<MaterialsBehavior>)playable.GetInput(i);
//                 MaterialsBehavior behavior = inputPlayable.GetBehaviour();
//                 if (behavior.materialPropertis.Count > 0)
//                 {
//                     if (i == 0)
//                     {
//                         materialPropertisTemp = behavior.materialPropertis;

//                         foreach (var item in materialPropertisTemp)
//                         {
//                             switch (item.paramType)
//                             {
//                                 case MatPropType.ColorT:
//                                     item.colorProp = Color.clear;
//                                     break;
//                                 case MatPropType.floatT:
//                                     item.floatProp = 0;
//                                     break;
//                                 case MatPropType.intT:
//                                     item.intProp = 0;
//                                     break;
//                                 default:
//                                     break;
//                             }
//                         }
//                     }

//                     for (int j = 0; j < behavior.materialPropertis.Count; j++)
//                     {
//                         switch (behavior.materialPropertis[j].paramType)
//                         {
//                             case MatPropType.ColorT:
//                                 materialPropertisTemp[j].colorProp += behavior.materialPropertis[j].colorProp * inputWeight;
//                                 break;
//                             case MatPropType.floatT:
//                                 materialPropertisTemp[j].floatProp += behavior.materialPropertis[j].floatProp * inputWeight;
//                                 break;
//                             case MatPropType.intT:
//                                 break;
//                             default:
//                                 break;
//                         }
//                     }
//                 }

//                 totalWeight += inputWeight;
//             }

//             float remainmingWeight = 1 - totalWeight;

//             for (int j = 0; j < materialPropertisTemp.Count; j++)
//             {
//                 switch (materialPropertisTemp[j].paramType)
//                 {
//                     case MatPropType.ColorT:
//                         material.SetColor(materialPropertisTemp[j].propertyName, (materialPropertisTemp[j].colorProp + materialPropertisDefault[j].colorProp * remainmingWeight));
//                         break;
//                     case MatPropType.floatT:
//                         material.SetFloat(materialPropertisTemp[j].propertyName, (materialPropertisTemp[j].floatProp + materialPropertisDefault[j].floatProp * remainmingWeight));
//                         break;
//                     default:
//                         break;
//                 }
//             }
//         }


//         public override void OnPlayableDestroy(Playable playable)
//         {
//             firstFrameHappened = false;

//             if (material == null)
//                 return;

//             foreach (var item in materialPropertisDefault)
//             {
//                 switch (item.paramType)
//                 {
//                     case MatPropType.ColorT:
//                         material.SetColor(item.propertyName, item.colorProp);
//                         break;
//                     case MatPropType.TextureT:
//                         material.SetTexture(item.propertyName, item.textureProp);
//                         break;
//                     case MatPropType.floatT:
//                         material.SetFloat(item.propertyName, item.floatProp);
//                         break;
//                     case MatPropType.intT:
//                         material.SetInt(item.propertyName, item.intProp);
//                         break;
//                     default:
//                         break;
//                 }
//             }

//         }
//     }
// }