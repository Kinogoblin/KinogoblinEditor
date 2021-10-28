using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    public class  KeyValueFavoriteSceneObject :  KeyValueFavorite
    {
        public  KeyValueFavoriteSceneObject(string typeName, List<string> list, Texture icon, bool isFolder)
        {
            base.m_key = typeName;
            base.m_favorites = list;
            base.m_icon = icon;
            base.m_isFolder = isFolder;
        }

        override internal void sortList(){}

        override internal void UpdateFilteredList(){}
    }
}