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

        public override void OnActive(Player player)
        {
            base.OnActive(player);
            foreach (var item in items)
            {
                Debug.Log("Initialize");
                item.Initialize(player);
            }
        }

        public override void OnInActive(Player player)
        {
            base.OnInActive(player);
            foreach (var item in items)
            {
                Debug.Log("Dispose");
                item.DisposeActiveStream();
                item.OnInActive(player);
            }
        }
    }
}