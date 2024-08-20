using UnityEngine;

namespace Helpers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance;
        public bool DontDestroy;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                if (DontDestroy)
                    DontDestroyOnLoad(this);
            }
            else if (Instance != this && DontDestroy)
            {
                Destroy(this);
            }
        }
    }
}
