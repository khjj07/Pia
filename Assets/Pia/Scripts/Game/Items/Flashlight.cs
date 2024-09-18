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
            _lightComponent = GetComponent<Light>();
            _lightComponent.enabled = false;
        }
        public override void OnHold(Player player)
        {
            _lightComponent.enabled = true;
        }
        public override void OnStopHold()
        {
            _lightComponent.enabled = true;
        }
    }
}