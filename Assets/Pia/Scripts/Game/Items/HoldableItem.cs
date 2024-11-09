using System;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UniRx;

namespace Assets.Pia.Scripts.Game.Items
{
    public abstract class HoldableItem : Item
    {
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            gameObject.SetActive(true);
        }
        public override void OnInActive(Player player)
        {
            base.OnInActive(player);
            gameObject.SetActive(false);
            player.Hold(null);
        }
        public override void Initialize(Player player)
        {
            activeStream = CreateActiveStream()
                .Where(_ => StoryModeManager.IsInteractionActive())
                .Subscribe(_ =>
                {
                    if (player.Hold(this) == this)
                    {
                        OnActive(player);
                        CreateInActiveStream()
                            .TakeWhile(_ => _isActive)
                            .Take(1)
                            .Subscribe(_ =>
                            {
                                OnInActive(player);
                                player.Hold(null);
                            })
                            .AddTo(gameObject);
                    }
                }).AddTo(gameObject);
        }


    }
}