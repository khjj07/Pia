using Assets.Pia.Scripts.UI;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class SecondaryBag : Item
    {
        [Header("아이템")]
        public Item[] items;
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            foreach (var item in items)
            {
                item.Initialize(player);
            }
        }

        public override void OnInActive(Player player)
        {
            base.OnInActive(player);
            foreach (var item in items)
            {
                item.DisposeActiveStream();
                item.OnInActive(player);
            }
        }
    }
}