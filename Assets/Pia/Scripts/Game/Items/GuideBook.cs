using Default.Scripts.Sound;
using Default.Scripts.Util;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class GuideBook : HoldableItem
    {
        [SerializeField] private KeyCode previousKey;
        [SerializeField] private KeyCode nextKey;
        private GuideBookUI _guideBookUi;

        public void Start()
        {
            _guideBookUi = GetComponent<GuideBookUI>();
        }
        public override void OnActive(Player player)
        {
            base.OnActive(player);
           gameObject.SetActive(true);
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
        public override void OnInActive()
        {
            base.OnInActive();
            gameObject.SetActive(false);
        }
    }
}