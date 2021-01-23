using UnityEngine;

namespace AsyncAwaitUtil.Internal
{
    public class AsyncCoroutineRunner : MonoBehaviour
    {
        static AsyncCoroutineRunner _instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("AsyncCoroutineRunner")
                        .AddComponent<AsyncCoroutineRunner>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            // Don't show in scene hierarchy
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(this.gameObject);
        }
    }
}
