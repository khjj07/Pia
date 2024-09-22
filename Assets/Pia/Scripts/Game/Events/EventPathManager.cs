using Assets.Pia.Scripts.Path;
using Assets.Pia.Scripts.StoryMode.Walking;
using UnityEditor;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Events
{
#if UNITY_EDITOR
    [CustomEditor(typeof(EventPathManager))]
    public class EventPathManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Register Path Node"))
            {
                var wm = (EventPathManager)target;
                wm.nodes = wm.GetComponentsInChildren<EventPathNode>(true);
            }
        }

        public virtual void OnSceneGUI()
        {
            var wm = (EventPathManager)target;
            var sceneView = SceneView.lastActiveSceneView;
            var rot = Quaternion.LookRotation(sceneView.camera.transform.forward, sceneView.camera.transform.up);

            for (var i = 0; i < wm.nodes.Length; i++)
            {

                var position = wm.nodes[i].transform.position;
                Handles.color = Color.white;

                if (Handles.Button(position, rot, 1, 1, Handles.RectangleHandleCap))
                    Selection.activeObject = wm.nodes[i].gameObject;
            }
        }
    }
#endif
    public class EventPathManager : MonoBehaviour
    {
        public EventPathNode[] nodes;
        private EventPathNode currentNode = null;
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (nodes.Length > 0)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(nodes[0].transform.position, 0.1f);
                for (int i = 1; i < nodes.Length; i++)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(nodes[i].transform.position, 0.1f);
                    Gizmos.color = Color.red;
                    var from = nodes[i - 1].transform.position;
                    var to = nodes[i].transform.position;
                    var direction = Vector3.Normalize(to - from);
                    Gizmos.DrawLine(from, to);
                    Handles.color = Color.cyan;
                    Handles.ArrowHandleCap(0, from, Quaternion.LookRotation(direction), 0.5f, EventType.Repaint);
                }
            }
        }
#endif
    }
}