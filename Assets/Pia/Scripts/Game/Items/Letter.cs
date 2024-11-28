using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Letter : HoldableItem
    {
        private Image _image;
        [SerializeField] private float fadeDuration = 1.0f;
        private Tween _tween=null;
        public void Awake()
        {
            _image = GetComponent<Image>();
        }
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            _image.DOKill();
            _image.color = new Color(1,1,1,0);
            _tween = _image.DOFade(1.0f, fadeDuration).OnComplete(() =>
            {
                Debug.Log("ActiveEnd");
            });
        }
        public override void OnInActive(Player player)
        {
            base.OnInActive(player);
            _image.DOKill();
            gameObject.SetActive(false);
        }
    }
}