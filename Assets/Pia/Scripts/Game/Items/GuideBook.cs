using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using System;
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
        [SerializeField] private float fadeDuration = 1.0f;
        private Tween _tween;
        public int index = 0;
        private IDisposable nextStream;
        private IDisposable prevStream;

        public void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = initialPosition;
        }

        public override void OnActive(Player player)
        {
            base.OnActive(player);
            _canvasGroup.DOKill();
            _guideBookUi = GetComponentInChildren<GuideBookUI>(true);
            _guideBookUi.Change(index);
            _rectTransform.anchoredPosition = initialPosition;
            _tween = _rectTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutBack);
            nextStream = GlobalInputBinder.CreateGetKeyDownStream(nextKey).Subscribe(_ =>
           {
               if (_guideBookUi.currentIndex < _guideBookUi.states.Length - 1)
               {
                   SoundManager.Play("use_book", 1);
                   _rectTransform.eulerAngles = Vector3.zero;
                   _rectTransform.DOShakeRotation(0.1f, 2, 0).SetEase(Ease.InOutElastic);
               }
               _guideBookUi.Next();
           }).AddTo(gameObject);
            prevStream = GlobalInputBinder.CreateGetKeyDownStream(previousKey).Subscribe(_ =>
            {
                 
                if (_guideBookUi.currentIndex > 0)
                {
                    SoundManager.Play("use_book", 1);
                    _rectTransform.eulerAngles = Vector3.zero;
                    _rectTransform.DOShakeRotation(0.1f, 2, 0).SetEase(Ease.InOutElastic);
                }
                _guideBookUi.Previous();
            }).AddTo(gameObject);

        }
        public override void OnInActive(Player player)
        {
            index = _guideBookUi.currentIndex;
            base.OnInActive(player);
            _canvasGroup.DOKill();
            nextStream.Dispose();
            prevStream.Dispose();
          
        }
    }
}