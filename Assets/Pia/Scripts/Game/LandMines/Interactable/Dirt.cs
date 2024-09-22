using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.Items;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using DG.Tweening;
using UnityEngine;

namespace Assets.Pia.Scripts.Effect
{
    public class Dirt : InteractableClass
    {
        [Range(0.0f, 1.0f)]
        public float holeDepth;

        [SerializeField] private float extend = 0.25f;

        private MeshRenderer _dirt;

        public Transform top;
        public Transform bottom;
        public float worldHeight;
        private float initialHoleDepth;
        public float targetDepth;

        public override void OnHover(Item item)
        {
            if (item is Shovel)
            {
          
            }
        }

        public override void OnExit()
        {
           
        }

        public override void Start()
        {
            _dirt = GetComponentInParent<MeshRenderer>();
            holeDepth = _dirt.sharedMaterial.GetFloat("_HoleDepth");
            worldHeight = top.position.y - bottom.position.y;
            initialHoleDepth = holeDepth;
            _isAvailable = true;
            GetComponentInParent<Collider>().enabled = _isAvailable;
        }

        public void Update()
        {
            top.position = bottom.position + Vector3.up * worldHeight * (1 - holeDepth);
            _dirt.material.SetFloat("_HoleDepth", holeDepth);
        }

        public void Dig(float interval)
        {
            holeDepth = Mathf.Clamp(holeDepth + extend, 0, 1);

            transform.DOShakeRotation(interval, 0.1f, 1);
            if (holeDepth >= targetDepth)
            {
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
