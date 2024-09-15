using UnityEngine;

namespace Default.Scripts.Debug
{
#if UNITY_EDITOR
    public class DebugCubeDrawer : MonoBehaviour
    {
        [SerializeField] private Color _color = Color.red;
        [SerializeField] public Vector3 _scale = Vector3.one;

        [SerializeField] private bool _wireFrame;
        // Update is called once per frame

        private void OnDrawGizmos()
        {
            var scale = Vector3.zero;
            scale.x = transform.localScale.x * _scale.x;
            scale.y = transform.localScale.y * _scale.y;
            scale.z = transform.localScale.z * _scale.z;
            Gizmos.color = _color;
            if (_wireFrame)
                Gizmos.DrawWireCube(transform.position, scale);
            else
                Gizmos.DrawCube(transform.position, scale);
        }
    }
#endif
}