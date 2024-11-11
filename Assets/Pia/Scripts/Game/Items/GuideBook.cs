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
        private GuideBookUI _guideBookUi;
        private CanvasGroup _canvasGroup;
        [SerializeField] private float fadeDuration=1.0f;
        private Tween _tween;

        public void Awake()
        {
            _guideBookUi = GetComponent<GuideBookUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0;
            _tween =  _canvasGroup.DOFade(1.0f, fadeDuration);
            var nextStream = GlobalInputBinder.CreateGetKeyDownStream(nextKey).Subscribe(_ =>
           {
               if (_guideBookUi.currentIndex < _guideBookUi.states.Length-1)
               {
                   SoundManager.Play("use_book",1);
               }
               _guideBookUi.Next();
           }).AddTo(gameObject);
           var prevStream = GlobalInputBinder.CreateGetKeyDownStream(previousKey).Subscribe(_ =>
           {
               if (_guideBookUi.currentIndex > 0)
               {
                   SoundManager.Play("use_book", 1);
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
            gameObject.SetActive(true);
            _canvasGroup.DOKill();
            _tween = _canvasGroup.DOFade(0.0f, fadeDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}