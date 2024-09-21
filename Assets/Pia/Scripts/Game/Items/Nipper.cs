﻿using System;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Nipper : UsableItem
    {
        public override void OnUse(Player player)
        {
            if (player.target is Spring spring)
            {
                player.SetCursorLocked();
                spring.Initialize();
                var cutStream = GlobalInputBinder.CreateGetKeyDownStream(useKey)
                    .TakeWhile(_=>spring.progress < 1)
                    .Subscribe(_ =>
                    {
                        spring.TryToCut();
                    },null, () =>
                    {
                        spring.CheckFinish();
                        player.SetCursorUnlocked();
                    }).AddTo(gameObject);

                player.UpdateAsObservable().Where(_ => !_isHold)
                    .Take(1).Subscribe(_=>
                    {
                        player.SetCursorUnlocked();
                        spring.Cancel();
                        cutStream.Dispose();
                    });
            }
        }
    }
}