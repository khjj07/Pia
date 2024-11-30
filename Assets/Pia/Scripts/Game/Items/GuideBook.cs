using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Game.Items
{
    public class GuideBook : HoldableItem
    {
        [SerializeField] private KeyCode previousKey;
        [SerializeField] private KeyCode nextKey;
        [SerializeField] private Vector3 initialPosition;

        private GuideBookUI _guideBookUi;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        [SerializeField] private float fadeDuration=1.0f;
        private Tween _tween;

        public void Awake()
        {
            _guideBookUi = GetComponent<GuideBookUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            _canvasGroup.DOKill();
            _rectTransform.anchoredPosition = initialPosition;
            _tween = _rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack);
            var nextStream = GlobalInputBinder.CreateGetKeyDownStream(nextKey).Subscribe(_ =>
           {
               if (_guideBookUi.currentIndex < _guideBookUi.states.Length-1)
               {
                   SoundManager.Play("use_book",1);
                   _rectTransform.DOShakeRotation(0.1f, 2,0).SetEase(Ease.InOutElastic);
               }
               _guideBookUi.Next();
           }).AddTo(gameObject);
           var prevStream = GlobalInputBinder.CreateGetKeyDownStream(previousKey).Subscribe(_ =>
           {
               if (_guideBookUi.currentIndex > 0)
               {
                   SoundManager.Play("use_book", 1);
                   _rectTransform.DOShakeRotation(0.1f, 2, 0).SetEase(Ease.InOutElastic);
               }
               _guideBookUi.Previous();
           }).AddTo(gameObject);
           player.UpdateAsObservable().Where(_ => !_isActive).Take(1).Subscribe(_ =>
           {
               nextStream.Dispose();
               prevStream.Dispose();
           }).AddTo(gameObject);
        }
        public override void OnInActive(Player player)
        {
            base.OnInActive(player);
            _canvasGroup.DOKill(); 
            gameObject.SetActive(false);
        }
    }
}