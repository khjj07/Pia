using System;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public abstract class UsableItem : Item
    {
        [SerializeField]
        protected KeyCode useKey = KeyCode.Mouse0;

        public IObservable<Unit> CreateUseStream()
        {
            return GlobalInputBinder.CreateGetKeyDownStream(useKey)
                .TakeUntil(CreateStopHoldStream());
        }
        public IObservable<Unit> CreateStopUseStream()
        {
            return GlobalInputBinder.CreateGetKeyUpStream(useKey);
        }



        public override void Initialize(Player player)
        {
            gameObject.SetActive(false);
            holdStream = CreateHoldStream()
                .SkipWhile(_ => !StoryModeManager.IsInteractionActive())
                .Subscribe(_ =>
                {
                    if (player.Hold(this) == this)
                    {
                        OnHold(player);
                        CreateUseStream()
                            .TakeWhile(_ => _isHold)
                            .TakeUntil(CreateStopHoldStream())
                            .Where(_ => player.IsInteractable())
                            .Subscribe(_ =>
                            {
                                OnUse(player);
                            })
                            .AddTo(player.gameObject);

                        CreateStopHoldStream()
                            .Subscribe(_ =>
                            {
                                player.Hold(null);
                                OnStopHold();
                            })
                            .AddTo(player.gameObject);
                    }
                }).AddTo(player.gameObject);
        }
        public abstract void OnUse(Player player);
    }
}