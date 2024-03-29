﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Kinogoblin.Editor
{
    //[CreateAssetMenu(fileName = "Editor Data", menuName = "Kinogoblin/Data ", order = 2)]
    public class SaveSettings : ScriptableObject
    {
        [SerializeField]
        public HierarchyCustomColors[] customHierarchy = null;
        public bool customizeHierarchy = true;

        public Color debugColor = new Color(0.5f, 0, 1);

        public bool customView = true;
        public bool debugSend = true;
        public string pathForModels = "Assets/__Project__/Models/MeshAssets/";
        public string pathForMaterials = "Assets/__Project__/Materials/";

        public GameObjectCustomSettings gameObjectCustomSettings;
        public MeshRenderer renderer;
        public bool enableCustomImportProcessor = false;

    }

    [Serializable]
    public class HierarchyCustomColors
    {
        public string prefix;
        public Color color = new Color(0.5f, 0, 1);
        public Color colorDisable = new Color(0, 0, 0, 0.5f);
        public GUIStyle style;
    }
    [Serializable]
    public class GOWithMissingScripts
    {
        public List<GameObject> gOWithMissingScripts = new List<GameObject>();
    }

    [Serializable]
    public class GameObjectCustomSettings
    {
        public GameObject renderer;
    }

}
