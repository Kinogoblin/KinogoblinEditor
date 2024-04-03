using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Runtime
{
    public class Helpful
    {
        private static Color debugColor = new Color(0.5f, 0, 1);
        private static string _toolMessage = "KinogoblinTool ";

        public static void Debug(string message, string text)
        {
            UnityEngine.Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(debugColor.r * 255f),
                (byte)(debugColor.g * 255f), (byte)(debugColor.b * 255f), message) + text);
        }

        public static void ToolDebug(string text)
        {
            Debug(_toolMessage, text);
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
        
        /// <summary>
        /// Returns the distance to a line segment. Based on the version outlined in
        /// Realtime Collision Detection by Christer Ericson.
        /// </summary>
        /// <param name="point">Point to test against</param>
        /// <param name="lineA">Start of line segment</param>
        /// <param name="lineB">End of line segment.</param>
        /// <returns>The square of the distance.</returns>
        public static float DistanceToLineSegmentSquared(Vector2 point, Vector2 lineA, Vector2 lineB)
        {
            Vector2 ab = lineB - lineA, ac = point - lineA, bc = point - lineB;
            var e = Vector2.Dot(ac, ab);

            // Handle cases where c projects outside ab
            if (e <= 0.0f) return Vector2.Dot(ac, ac);
            var f = Vector2.Dot(ab, ab);
            if (e >= f) return Vector2.Dot(bc, bc);

            // Handle cases where c projects onto ab
            return Vector2.Dot(ac, ac) - e * e / f;
        }
        
        // https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/ 
        public static Vector3 NearestPointOnLine(Vector3 linePoint, Vector3 lineDirection, Vector3 queryPoint)
        {
            lineDirection.Normalize();//this needs to be a unit vector
            var v = queryPoint - linePoint;
            var d = Vector3.Dot(v, lineDirection);
            return linePoint + lineDirection * d;
        }

        // https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/ 
        public static Vector3 NearestPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 queryPoint)
        {
            var lineDirection = lineEnd - lineStart;
            var magnitude = lineDirection.magnitude;
            lineDirection.Normalize();

            var v = queryPoint - lineStart;
            var d = Vector3.Dot(v, lineDirection);
            d = Mathf.Clamp(d, 0f, magnitude);
            return lineStart + lineDirection * d;
        }

        //Based on: https://www.topcoder.com/thrive/articles/Geometry%20Concepts%20part%202:%20%20Line%20Intersection%20and%20its%20Applications 
        public static bool SegmentLineIntersection(Vector2 segmentStart, Vector2 segmentEnd, Vector2 lineStart, Vector2 lineEnd,
            out Vector2 result)
        {
            var a1 = segmentEnd.y - segmentStart.y;
            var b1 = segmentStart.x - segmentEnd.x;
            var c1 = a1 * segmentStart.x + b1 * segmentStart.y;

            var a2 = lineEnd.y - lineStart.y;
            var b2 = lineStart.x - lineEnd.x;
            var c2 = a2 * lineStart.x + b2 * lineStart.y;

            var det = a1 * b2 - a2 * b1;

            if (det == 0)
            {
                //Lines are parallel
                result = Vector2.zero;
                return false;
            }

            var x = (b2 * c1 - b1 * c2) / det;
            var y = (a1 * c2 - a2 * c1) / det;

            if (x > Mathf.Max(segmentStart.x, segmentEnd.x) ||
                x < Mathf.Min(segmentStart.x, segmentEnd.x) ||
                y > Mathf.Max(segmentStart.y, segmentEnd.y) ||
                y < Mathf.Min(segmentStart.y, segmentEnd.y))
            {
                result = Vector2.zero;
                return false;
            }

            if (x > Mathf.Max(lineStart.x, lineEnd.x) ||
                x < Mathf.Min(lineStart.x, lineEnd.x) ||
                y > Mathf.Max(lineStart.y, lineEnd.y) ||
                y < Mathf.Min(lineStart.y, lineEnd.y))
            {
                result = Vector2.zero;
                return false;
            }

            result = new Vector2(x, y);
            return true;
        }
    }
}