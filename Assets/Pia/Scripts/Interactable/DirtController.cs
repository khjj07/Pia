using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using DG.Tweening;
using UnityEngine;

namespace Assets.Pia.Scripts.Effect
{
    public class DirtController : InteractableClass
    {
        [Range(0.0f, 1.0f)]
        public float holeDepth;
        [SerializeField]
        [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
        private Color defaultColor;
        [SerializeField]
        [ColorUsage(true, true, 0f, 8f, 0.125f, 3f)]
        private Color highlightColor;

        [SerializeField] private float extend = 0.25f;

        private MeshRenderer _dirt;

        public Transform top;
        public Transform bottom;
        public float worldHeight;
        private float initialHoleDepth;

        public override void OnInteract(object interval)
        {
            Dig((float)interval);   
        }

        public override void OnHover(Player.UpperState state)
        {
            if (state == Player.UpperState.Shovel || state == Player.UpperState.ShovelUse)
            {
                HightLightOn();
            }
            else
            {
                HightLightOff();
            }
        }

        public override void OnExit()
        {
            HightLightOff();
        }

        public override void Start()
        {
            _dirt = GetComponent<MeshRenderer>();
            holeDepth = _dirt.sharedMaterial.GetFloat("_HoleDepth");
            worldHeight = top.position.y - bottom.position.y;
            initialHoleDepth = holeDepth;
            _isAvailable = true;
            GetComponent<Collider>().enabled = _isAvailable;
        }

        public void Update()
        {
            top.position = bottom.position + Vector3.up * worldHeight * (1 - holeDepth);
            _dirt.material.SetFloat("_HoleDepth", holeDepth);
        }

        public void HightLightOn()
        {
            _dirt.sharedMaterial.SetColor("_Highlight", highlightColor);
        }
        public void Dig(float interval)
        {
            holeDepth = Mathf.Clamp(holeDepth + extend, 0, 1);

            transform.DOShakeRotation(interval, 0.1f, 1);
            if (holeDepth == 1)
            {
                gameObject.SetActive(false);
            }
        }
        public void HightLightOff()
        {
            _dirt.sharedMaterial.SetColor("_Highlight", defaultColor);
        }
    }
}
