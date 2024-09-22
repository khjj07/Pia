using System;
using Assets.Pia.Scripts.Interface;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Driver : UsableItem
    {
        [SerializeField]
        private float screwInterval;

        public override void OnUse(Player player)
        {
            if (player.target is Bolt bolt)
            {
                Observable.Interval(TimeSpan.FromSeconds(screwInterval))
                    .TakeUntil(CreateStopUseStream())
                    .TakeWhile(_ => _isHold && player.target == bolt)
                    .Subscribe(_ => bolt.Screw(screwInterval));
            }
        }
    }
}