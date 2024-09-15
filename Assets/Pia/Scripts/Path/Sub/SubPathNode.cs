using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Pia.Scripts.Path
{
    public abstract class SubPathNode : MonoBehaviour
    {
        public float appearDelay;
        public bool clearPreviousNode = false;

        public abstract Task Appear();
        public abstract Task Disappear();
    }
}