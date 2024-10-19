using Assets.Pia.Scripts.StoryMode.Synopsis.Sub;
using System.Threading.Tasks;
using Default.Scripts.Util;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Path
{
    [RequireComponent(typeof(SphereCollider))]
    public class PathNode : MonoBehaviour
    {
        public bool stopAtNode = false;
        [SerializeField]
        private SubPathNode[] _subNodes;
        private bool _isPrinting;
        private Canvas canvas;
        public virtual void Start()
        {
            canvas = GetComponentInChildren<Canvas>(true);
            _subNodes = GetComponentsInChildren<SubPathNode>(true);
            foreach (var state in _subNodes)
            {
                state.gameObject.SetActive(false);
            }
            canvas.gameObject.SetActive(false);
        }
        public async Task Print()
        {
            canvas.gameObject.SetActive(true);
            _isPrinting = true;
            foreach (var node in _subNodes)
            {
                
                await Task.Delay((int)(node.appearDelay*1000));
                if (node.clearPreviousNode)
                {
                    await ClearPreviousSubNode();
                }
                await node.Appear();
                await Task.Delay((int)(node.duration * 1000));
            }
            _isPrinting = false;
        }

        public async Task ClearPreviousSubNode()
        {
            var tmp = GetComponentsInChildren<SubPathNode>();
            foreach (var node in tmp)
            {
                await node.Disappear();
            }
        }

        public async Task Disappear()
        {
            canvas.gameObject.SetActive(true);
            _isPrinting = true;
            foreach (var node in _subNodes)
            {
                await node.Disappear();
            }
            _isPrinting = false;
        }
        public bool IsPrinting()
        {
            return _isPrinting;
        }

        public void SetStopAtNode(bool value)
        {
            stopAtNode = value;
        }

    }
}