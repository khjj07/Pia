using Assets.Pia.Scripts.StoryMode;
using Knife.HDRPOutline.Core;
using UnityEngine;

namespace Assets.Pia.Scripts.Interface
{
    public abstract class InteractableClass : MonoBehaviour
    {
        public abstract void OnInteract(object player);
        protected bool _isAvailable = false;
        public bool isDead = false;
        [SerializeField]
        protected OutlineController availableOutline;
        [SerializeField]
        protected OutlineController inavailableOutline;
        public void Awake()
        {

        }
        public virtual void SetAvailable(bool value)
        {
            _isAvailable = value;
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }
        public virtual void OnHover(Player.UpperState state)
        {
            
        }

        public virtual void OnExit()
        {
            availableOutline.gameObject.SetActive(false);
            inavailableOutline.gameObject.SetActive(false);
        }
    }
}