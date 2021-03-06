﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Kinogoblin
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

    }

    [Serializable]
    public class HierarchyCustomColors
    {
        public string prefix;
        public Color color = new Color(0.5f, 0, 1);
        public GUIStyle style;
    }
}
