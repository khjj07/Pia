﻿using System;
using Assets.Pia.Scripts.Effect;
using Assets.Pia.Scripts.Interface;
using Default.Scripts.Sound;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Shovel : UsableItem
    {
        public Animator animator;
        [SerializeField] 
        private float digInterval = 1;
        private float soundInterval = 17;
        public override void OnUse(Player player)
        {
            if (player.target is Dirt dirt)
            {
                animator.SetBool("Use", true);
                Observable.Interval(TimeSpan.FromSeconds(digInterval))
                    .TakeUntil(CreateStopUseStream())
                    .TakeWhile(_ => _isHold)
                    .TakeWhile(_ => player.target == dirt)
                    .Subscribe(_ =>
                    {
                        dirt.Dig(digInterval);
                        animator.SetBool("Use", true);
                    },null, () =>
                    {
                        animator.SetBool("Use", false);
                    }).AddTo(player.gameObject);

                CreateStopUseStream().Take(1)
                    .Subscribe(_ => animator.SetBool("Use", false))
                    .AddTo(player.gameObject);

                SoundManager.Play("use_shovelDig", 1);
                Observable.Interval(TimeSpan.FromSeconds(soundInterval))
                    .TakeUntil(CreateStopUseStream())
                    .TakeWhile(_ => _isHold)
                    .TakeWhile(_ => player.target is Dirt)
                    .Subscribe(_ => SoundManager.Play("use_shovelDig", 1), null, () =>
                    {
                        SoundManager.Stop(1);
                    }).AddTo(gameObject);
            }
            
        }
    }
}