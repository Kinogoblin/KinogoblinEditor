using System.Collections.Generic;
using UnityEngine;

namespace Kinogoblin.Runtime
{
    public class UserDataHolder : MonoBehaviour
    {
        public List<UserDataFromModel> UserData = new List<UserDataFromModel>();
    }

    [System.Serializable]
    public class UserDataFromModel
    {
        public string name;
        public string type;
        public string value;
    }
}