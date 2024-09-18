using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    [RequireComponent(typeof(Image))]
    public class StoryBoardSubImageState : StoryBoardSubState
    {
        private Image _image;
        [SerializeField] private Color beginColor = new Color();
        [SerializeField] private Color endColor = Color.white;
        [SerializeField] private float duration = 0.5f;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = beginColor;
        }

        public override async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
            gameObject.SetActive(true);
            _image.DOColor(endColor, duration);
        }
    }
}