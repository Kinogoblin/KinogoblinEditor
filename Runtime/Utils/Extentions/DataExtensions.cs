﻿using UnityEngine;

namespace Kinogoblin.Runtime
{
    public static class DataExtensions
    {
        public static Vector3Data AsVectorData(this Vector3 vector) =>
            new Vector3Data(vector.x, vector.y, vector.z);
        public static Vector3 AsUnityVector(this Vector3Data vector3Data) =>
            new Vector3(vector3Data.X, vector3Data.Y, vector3Data.Z);

        public static Vector3 AddX(this Vector3 vector, float x)
        {
            vector.x += x;
            return vector;
        }

        public static Vector3 AddY(this Vector3 vector, float y)
        {
            vector.y += y;
            return vector;
        }
        
        public static Vector3 AddZ(this Vector3 vector, float z)
        {
            vector.z += z;
            return vector;
        }

        public static string ToJson(this object obj) =>
            JsonUtility.ToJson(obj);
        public static T ToDeserialized<T>(this string json) =>
            JsonUtility.FromJson<T>(json);
    }
}