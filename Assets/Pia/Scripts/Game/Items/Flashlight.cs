using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Flashlight : Item
    {
        private Light _lightComponent;

        public void Start()
        {
            gameObject.SetActive(false);
        }
        public override void OnHold(Player player)
        {
            base.OnHold(player);
            gameObject.SetActive(true);
        }
        public override void OnStopHold()
        {
            base.OnStopHold();
            gameObject.SetActive(false);
        }
    }
}