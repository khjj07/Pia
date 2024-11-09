using System;
using Assets.Pia.Scripts.Interface;
using Default.Scripts.Sound;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Driver : UsableItem
    {
        [SerializeField]
        private float screwInterval;
     
        private float soundInterval = 1;


        public override void OnUse(Player player)
        {
            if (player.target is Bolt bolt)
            {
                Observable.Interval(TimeSpan.FromSeconds(screwInterval))
                    .TakeUntil(CreateStopUseStream())
                    .TakeWhile(_ => _isHold && player.target == bolt)
                    .Subscribe(_ =>
                    {
                        bolt.Screw(screwInterval);
                    });

                SoundManager.Play("use_driverScrew", 1);
                Observable.Interval(TimeSpan.FromSeconds(soundInterval))
                    .TakeUntil(CreateStopUseStream())
                    .TakeWhile(_ => _isHold && player.target == bolt)
                    .Subscribe(_ => SoundManager.Play("use_driverScrew", 1), null, () =>
                    {
                        SoundManager.Stop(1);
                    }).AddTo(gameObject);
                     
            }
        }
    }
}