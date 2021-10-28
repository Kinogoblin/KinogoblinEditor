using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class PackageDataManager : ScriptableObject
    {
        public Texture2D icon;
        public string title;
        public List<PackageLinks> Links = new List<PackageLinks>();

        public PackageData[] sections;
        public bool loadedLayout;
    }
}