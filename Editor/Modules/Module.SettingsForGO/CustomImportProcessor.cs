using UnityEngine;
using UnityEditor;
using System.IO;
using Kinogoblin.Runtime;
using System.Collections.Generic;

namespace Kinogoblin
{
    class CustomImportProcessor : AssetPostprocessor
    {
        void OnPostprocessGameObjectWithUserProperties(GameObject go, string[] names, System.Object[] values)
        {
            if (Other.settings.enableCustomImportProcessor)
            {
                ModelImporter importer = (ModelImporter)assetImporter;
                var asset_name = Path.GetFileName(importer.assetPath);
                Debug.LogFormat("OnPostprocessGameObjectWithUserProperties(go = {0}) asset = {1}", go.name, asset_name);
                string str = null;
                Vector3 vec3 = Vector3.zero;
                float valFloat = 0;
                List<UserDataFromModel> dataList = new List<UserDataFromModel>();

                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    var val = values[i];
                    var type = val.GetType().Name;
                    UserDataFromModel data = new UserDataFromModel();
                    data.name = name;
                    data.type = type;
                    data.value = val.ToString();
                    dataList.Add(data);
                    // switch (name)
                    // {
                    //     case "StringData":
                    //         str = (string)val;
                    //         break;
                    //     case "test_v1":
                    //         valFloat = (float)val;
                    //         break;
                    //     default:
                    //         Debug.LogFormat("Unknown Property : {0} : {1} : {2}", name, val.GetType().Name, val.ToString());
                    //         break;
                    // }
                }
                if (dataList.Count != 0)
                {
                    var udh = go.AddComponent<UserDataHolder>();
                    udh.UserData = dataList;
                }
            }
        }
    }
}