using System;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Nipper : UsableItem
    {

        public override void OnStopHold()
        {
            gameObject.SetActive(false);
        }
        public override void OnUse(Player player)
        {
            if (player.target is Spring spring)
            {
                spring.Initialize(this.UpdateAsObservable().Where(_ => !_isHold));
                player.SetCursorLocked();
                GlobalInputBinder.CreateGetKeyDownStream(useKey)
                    .TakeWhile(_=>_isHold)
                    .TakeWhile(_ => spring.progress < 1)
                    .Subscribe(_ =>
                    {
                        spring.TryToCut();
                    }, null, () =>
                    {
                        player.SetCursorUnlocked();
                        spring.CheckRemove();
                    });
            }
        }
    }
}