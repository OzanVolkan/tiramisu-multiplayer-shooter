using UnityEngine;

namespace Managers
{
    public class SingletonManager<T> : MonoBehaviour where T: SingletonManager<T>
    {
        private static volatile T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                }

                return instance;
            }
        }
    }
}
