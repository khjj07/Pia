using Assets.Pia.Scripts.UI;
using UnityEngine;
using static UnityEditor.Progress;

namespace Assets.Pia.Scripts.Game.Items
{
    public class SecondaryBag : Item
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