using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.StoryMode;
using Knife.HDRPOutline.Core;
using UnityEngine;

namespace Assets.Pia.Scripts.Interface
{
    public abstract class InteractableClass : MonoBehaviour
    {
        protected bool _isAvailable = false;
        public bool isDead = false;
        [SerializeField]
        protected OutlineController availableOutline;
        [SerializeField]
        protected OutlineController inavailableOutline;

        public virtual void Start()
        {
            GetComponent<Collider>().enabled = _isAvailable;
        }
        public virtual void SetAvailable(bool value)
        {
            _isAvailable = value;
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }
        public virtual void OnHover(Item item)
        {
            
        }

        public virtual void OnExit()
        {
            availableOutline.gameObject.SetActive(false);
            inavailableOutline.gameObject.SetActive(false);
        }
    }
}