using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Runtime
{
    public class Helpful
    {
        private static Color debugColor = new Color(0.5f, 0, 1);

        public static void Debug(string message, string normal)
        {
            UnityEngine.Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(debugColor.r * 255f),
                (byte)(debugColor.g * 255f), (byte)(debugColor.b * 255f), message) + normal);
        }

        public static void Debug(string message)
        {
            UnityEngine.Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(debugColor.r * 255f),
                (byte)(debugColor.g * 255f), (byte)(debugColor.b * 255f), message));
        }

        public static void Debug(string message, Color color)
        {
            UnityEngine.Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f),
                (byte)(color.g * 255f), (byte)(color.b * 255f), message));
        }

        public static string CustomString(string message, Color color)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f),
                (byte)(color.g * 255f), (byte)(color.b * 255f), message);
        }

        public static string CustomString(string message)
        {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(debugColor.r * 255f),
                (byte)(debugColor.g * 255f), (byte)(debugColor.b * 255f), message);
        }

        public static void GetListOfAllChilds(Transform parent, List<Transform> list)
        {
            foreach (Transform child in parent)
            {
                list.Add(child);
                GetListOfAllChilds(child, list);
            }
        }

        public static List<Transform> GetListOfAllChilds(Transform parent)
        {
            List<Transform> list = new List<Transform>();
            foreach (Transform child in parent)
            {
                list.Add(child);
                GetListOfAllChilds(child, list);
            }

            return list;
        }

        static void GetListOfChildsWithComponent<T>(Transform parent, List<Transform> list)
        {
            foreach (Transform child in parent)
            {
                if (child.GetComponent<T>() != null)
                {
                    list.Add(child);
                }

                GetListOfAllChilds(child, list);
            }
        }

        public static string GetName(string name, char symbol, int numberName)
        {
            string newname = null;
            if (name.Contains(symbol.ToString()))
                newname = name.Split(new char[] { symbol })[numberName];
            else
                newname = name;
            return newname;
        }

        public static string[] GetName(string name, char symbol)
        {
            string[] newname = null;
            if (name.Contains(symbol.ToString()))
                newname = name.Split(new char[] { symbol });
            return newname;
        }

        public static string[] GetName(string name, string textPart)
        {
            string[] newname = null;
            if (name.Contains(textPart))
                newname = name.Split(new string[] { textPart }, StringSplitOptions.None);
            return newname;
        }
    }
}