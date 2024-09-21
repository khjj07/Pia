using System;
using System.Collections.Generic;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.UI;
using Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Cartridge : Item
    {
   
        [Header("아이템")]
        public Item[] items;

        public override void OnHold(Player player)
        {
            base.OnHold(player);
            foreach (var item in items)
            {
                Debug.Log("Initialize");
                item.Initialize(player);
            }
        }

        public override void OnStopHold()
        {
            base.OnStopHold();
            foreach (var item in items)
            {
                Debug.Log("Dispose");
                item.DisposeHoldStream();
                item.OnStopHold();
            }
        }
    }
}