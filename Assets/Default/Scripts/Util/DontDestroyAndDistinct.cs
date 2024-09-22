using UnityEngine;

namespace Assets.Default.Scripts.Util
{
    public class DontDestroyAndDistinct : MonoBehaviour
    {
        private static DontDestroyAndDistinct _instance = null;
        void Awake()
        {
            if (_instance is null)
            {
                 _instance=this;
                 DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
