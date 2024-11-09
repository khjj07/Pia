﻿namespace Assets.Pia.Scripts.Game.Items
{
    public class Bottle : HoldableItem
    {
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            gameObject.SetActive(true);
        }
        public override void OnInActive()
        {
            base.OnInActive();
            gameObject.SetActive(false);
        }
    }
}