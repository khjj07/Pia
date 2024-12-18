using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Flashlight : Item
    {
        private Light _lightComponent;
        private float speed = 5f;

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

        public void Follow(Transform cameraTransform)
        {
             transform.position = Vector3.Lerp(transform.position, cameraTransform.position, Time.deltaTime * speed);
             Vector3 eulerAngle= Vector3.zero;
             eulerAngle.x = Mathf.LerpAngle(transform.eulerAngles.x, cameraTransform.eulerAngles.x, Time.deltaTime * speed);
             eulerAngle.y = Mathf.LerpAngle(transform.eulerAngles.y, cameraTransform.eulerAngles.y, Time.deltaTime * speed);
             eulerAngle.z = Mathf.LerpAngle(transform.eulerAngles.z, cameraTransform.eulerAngles.z, Time.deltaTime * speed);
             transform.eulerAngles = eulerAngle;
        }
    }
}