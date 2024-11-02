using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Pia.Scripts.Path
{
    public abstract class SubPathNode : MonoBehaviour
    {
        public float appearDelay;
        public float duration;
        public bool clearPreviousNode = false;

        public abstract Task Appear(CancellationTokenSource cancellationTokenSource);
        public abstract Task Disappear(CancellationTokenSource cancellationTokenSource);
    }
}