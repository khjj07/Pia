using System;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Assets.Pia.Scripts.UI;
using Assets.Pia.Scripts.UI.Slot;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField]
        private SlotUI slot;
        [SerializeField]
        protected KeyCode holdKey;

        protected bool _isHold=false;


        public IDisposable holdStream;
        public IObservable<Unit> CreateHoldStream()
        {
            return GlobalInputBinder.CreateGetKeyDownStream(holdKey);
        }
        public IObservable<Unit> CreateStopHoldStream()
        {
            return GlobalInputBinder.CreateGetKeyUpStream(holdKey);
        }

        public void DisposeHoldStream()
        {
            if (holdStream != null)
            {
                holdStream.Dispose();
            }
        }
        public virtual void OnHold(Player player)
        {
            _isHold = true; 
            slot.SetActive(true);
        }
        public virtual void OnStopHold()
        {
            _isHold = false;
            slot.SetActive(false);
        }

        public virtual void Initialize(Player player)
        {
           holdStream  = CreateHoldStream()
                .SkipWhile(_ => !StoryModeManager.IsInteractionActive())
                .Subscribe(_ =>
                {
                    //player.Hold(this);
                    OnHold(player);
                    CreateStopHoldStream()
                        .TakeWhile(_ => _isHold)
                        .Take(1)
                        .Subscribe(_ =>
                        {
                            player.Hold(null);
                            OnStopHold();
                        })
                        .AddTo(gameObject);

                }).AddTo(gameObject);
        }
    }
}