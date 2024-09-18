using System;
using Assets.Pia.Scripts.Effect;
using Assets.Pia.Scripts.Interface;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Shovel : UsableItem
    {
        public Animator animator;
        [SerializeField] 
        private float digInterval = 1;
        public override void OnUse(Player player)
        {
            if (player.target is Dirt dirt)
            {
                animator.SetBool("Use", true);
                Observable.Interval(TimeSpan.FromSeconds(digInterval))
                    .TakeUntil(CreateStopUseStream())
                    .TakeWhile(_ => _isHold)
                    .TakeWhile(_ => player.target is Dirt)
                    .Subscribe(_ =>
                    {
                        dirt.Dig(digInterval);
                        animator.SetBool("Use", true);
                    },null, () =>
                    {
                        animator.SetBool("Use", false);
                    }).AddTo(player.gameObject);

                CreateStopUseStream().First()
                    .Subscribe(_ => animator.SetBool("Use", false))
                    .AddTo(player.gameObject);
            }
            
        }
    }
}