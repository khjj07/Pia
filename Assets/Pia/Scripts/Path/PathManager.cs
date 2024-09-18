using System.Collections;
using System.Collections.Generic;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Path;
using Default.Scripts.Printer;
using Default.Scripts.Util.StatePattern;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Assets.Pia.Scripts.StoryMode.Walking
{
    [CustomEditor(typeof(PathManager))]
    public class WalkingManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Register Path Node"))
            {
                var wm = (PathManager)target;
                wm.nodes = wm.GetComponentsInChildren<PathNode>(true);
            }
        }

        public virtual void OnSceneGUI()
        {
            var wm = (PathManager)target;
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

    public class PathManager : MonoBehaviour
    {
        public PathNode[] nodes;
        private Queue<PathNode> appearQueue;

        [SerializeField] bool runPrintProcess = true;


        [SerializeField] float tolerance = 0.1f;
        int nextIndex = 0;
        private PathNode currentNode = null;

        private void Start()
        {
            appearQueue = new Queue<PathNode>();
            RegisterAllNode();
            StartCoroutine(PrintProcess());
        }

        public int CheckNode(Player player)
        {
            if (nodes.Length > nextIndex)
            {
                if (Vector3.Distance(player.transform.position, nodes[nextIndex].transform.position) < tolerance)
                {
                    nextIndex++;
                }

                return nextIndex;
            }
            else
            {
                return -1;
            }
        }

        public bool PlayerIsMovable()
        {
            if (nextIndex > 1)
            {
                return !nodes[nextIndex - 1].stopAtNode;
            }
            else
            {
                return true;
            }
        }

    public Vector3 GetPlayerDirection(Player player)
        {

            if (nodes.Length > nextIndex)
            {
                return Vector3.Normalize(nodes[nextIndex].transform.position - player.transform.position);
            }
            else
            {
                return Vector3.forward;
            }
        }

        private void RegisterAllNode()
        {
            foreach (var node in nodes)
            {
                node.OnTriggerEnterAsObservable().Subscribe(_ =>
                {
                    appearQueue.Enqueue(node);
                    node.GetComponent<SphereCollider>().enabled = false;
                });
            }
        }

        private IEnumerator PrintProcess()
        {
            while (runPrintProcess)
            {
                if (appearQueue.Count > 0)
                {
                    if (!currentNode)
                    {
                        Print(appearQueue.Dequeue());
                    }
                    else if (currentNode && !currentNode.IsPrinting())
                    {
                        Print(appearQueue.Dequeue());
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        private async void Print(PathNode item)
        {
            if (currentNode)
            {
                await currentNode.Disappear();
            }
            currentNode = item;
            await currentNode.Print();
        }

        public void OnDrawGizmos()
        {
            if (nodes.Length > 0)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(nodes[0].transform.position, 0.1f);
                for (int i = 1; i < nodes.Length; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(nodes[i].transform.position, 0.1f);
                    Gizmos.color = Color.blue;
                    var from = nodes[i - 1].transform.position;
                    var to = nodes[i].transform.position;
                    var direction = Vector3.Normalize(to - from);
                    Gizmos.DrawLine(from, to);
                    Handles.color = Color.yellow;
                    Handles.ArrowHandleCap(0, from, Quaternion.LookRotation(direction), 0.5f, EventType.Repaint);

                }
            }
        }
    }
}