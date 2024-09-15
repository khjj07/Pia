using UnityEngine;

namespace Default.Scripts.Debug
{
#if UNITY_EDITOR
    public class DebugSphereDrawer : MonoBehaviour
    {
        [SerializeField] private Color _color = Color.red;
        [SerializeField] public float size = 1.0f;

        [SerializeField] private bool _wireFrame;
        // Update is called once per frame

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            if (_wireFrame)
                Gizmos.DrawWireSphere(transform.position, size);
            else
                Gizmos.DrawSphere(transform.position, size);
        }
    }
#endif
}