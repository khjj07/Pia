using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Path;
using Default.Scripts.Printer;
using Default.Scripts.Util.StatePattern;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Pia.Scripts.StoryMode.Walking
{
#if UNITY_EDITOR
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
#endif

    public class PathManager : MonoBehaviour
    {
        [Header("Walk")]
        public PathNode[] nodes;
        int currentIndex = 0;

        private PathNode currentPrintNode = null;
        private Queue<PathNode> appearQueue;

        [SerializeField] bool runPrintProcess = true;


        [SerializeField] float tolerance = 0.1f;

        private void Start()
        {
            appearQueue = new Queue<PathNode>();
            RegisterAllNode();
            StartCoroutine(PrintProcess());
        }


        public bool PlayerIsMovable()
        {
            if (currentIndex > 1)
            {
                return !nodes[currentIndex].stopAtNode;
            }
            else
            {
                return true;
            }
        }

        public PathNode GetNext()
        {
            if (currentIndex < nodes.Length-1)
            {
                return nodes[currentIndex+1];
            }
            else
            {
                return null;
            }
        }

        public void Next()
        {
            currentIndex++;
        }

        public void StopPrintProcess()
        {
            runPrintProcess = false;
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
                    if (currentPrintNode && !currentPrintNode.IsPrinting())
                    {
                        Print(appearQueue.Dequeue());
                    }
                    else if(!currentPrintNode)
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
            if (currentPrintNode)
            {
                await currentPrintNode.Disappear(StoryModeManager.GetGameOverTokenSource());
            }
            currentPrintNode = item;
            await item.Print(StoryModeManager.GetGameOverTokenSource());
        }
#if UNITY_EDITOR
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
#endif
        public void UpdateCurrentNode(Vector3 position)
        {
            var next = GetNext();

            if (next != null)
            {
                if (Vector3.Distance(position, next.transform.position) < tolerance)
                {
                    currentIndex++;
                }
            }
        }
    }
}