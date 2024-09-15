using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class ArrowHighlighter : MonoBehaviour
    {
        public Vector2 direction;
        public float length;
        public float duration;
        public Ease easing;
        public void Start()
        {
            GetComponent<RectTransform>().DOAnchorPos(direction * length, duration).SetRelative().SetLoops(-1, LoopType.Yoyo).SetEase(easing);
        }
    }
}
