using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Kinogoblin.Runtime
{
    public abstract class Manager : MonoBehaviour
    {
        private static Dictionary<Type, Manager> s_Managers = new Dictionary<Type, Manager>();

        public static bool TryGet<T>(out T manager) where T: Manager
        {
            manager = null;
            if(s_Managers.ContainsKey(typeof(T)))
            {
                manager = (T)s_Managers[typeof(T)];
                return true;
            }
            else
                return false;
        }

        public static T Get<T>() where T: Manager
        {
            if(s_Managers.ContainsKey(typeof(T)))
                return (T)s_Managers[typeof(T)];
            else
            {
                Debug.LogError($"Manager of type '{typeof(T)}' could not be accessed. Check the excludedManagers list in your GameplayIngredientsSettings configuration file.");
                return null;
            }
        }

        public static bool Has<T>() where T:Manager
        {
            return(s_Managers.ContainsKey(typeof(T)));
        }

        static readonly Type[] kAllManagerTypes = TypeUtility.GetConcreteTypes<Manager>();

    }
}
