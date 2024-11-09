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
        public override void OnActive(Player player)
        {
            base.OnActive(player);
            gameObject.SetActive(true);
        }
        public override void OnInActive(Player player)
        {
            base.OnInActive(player);
            gameObject.SetActive(false);
        }
    }
}