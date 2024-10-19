using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    [RequireComponent(typeof(Image))]
    public class StoryBoardCustomTitleImageState : StoryBoardSubState
    {
        private Image _image;
        [SerializeField] private float duration = 0.5f;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public override async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
            gameObject.SetActive(true);
            _image.rectTransform.DOAnchorPos(Vector2.zero, duration);
            _image.rectTransform.DOScale(Vector2.one, duration);
        }
    }
}