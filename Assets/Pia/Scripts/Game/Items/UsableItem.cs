using System;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public abstract class UsableItem : HoldableItem
    {
        [SerializeField]
        protected KeyCode useKey = KeyCode.Mouse0;
        [SerializeField]
        private bool ignoreIsInteractive=false;
  

        public IObservable<Unit> CreateUseStream()
        {
            return GlobalInputBinder.CreateGetKeyDownStream(useKey);
        }
        public IObservable<Unit> CreateStopUseStream()
        {
            return GlobalInputBinder.CreateGetKeyUpStream(useKey);
        }

        public override void Initialize(Player player)
        {
            gameObject.SetActive(false);
            activeStream = CreateActiveStream()
                .Where(_ => StoryModeManager.IsInteractionActive())
                .Subscribe(_ =>
                {
                    if (player.Hold(this) == this)
                    {
                        OnActive(player);
                        CreateUseStream()
                            .TakeWhile(_ => _isActive)
                            .Where(_ => ignoreIsInteractive || player.IsInteractable())
                            .Subscribe(_ =>
                            {
                                OnUse(player);
                            })
                            .AddTo(player.gameObject);


                        CreateInActiveStream()
                            .TakeWhile(_ => _isActive)
                            .Take(1)
                            .Subscribe(_ =>
                            {
                                player.Hold(null);
                                OnInActive(player);
                            })
                            .AddTo(player.gameObject);
                    }
                }).AddTo(player.gameObject);
        }
        public abstract void OnUse(Player player);
    }
}