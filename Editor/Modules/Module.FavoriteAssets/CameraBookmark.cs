using UnityEngine;

namespace Kinogoblin.Editor.FavoriteAssets
{
    //A single saved Scene View camera pose.
    [System.Serializable]
    public class CameraBookmark
    {
        public Vector3 pivot;
        public Quaternion rotation;
        public float size;
        public bool orthographic;
    }
}
