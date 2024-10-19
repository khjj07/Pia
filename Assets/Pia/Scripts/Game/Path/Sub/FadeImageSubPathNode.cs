using System.Reflection;
using System.Threading.Tasks;
using Assets.Pia.Scripts.Path;
using Default.Scripts.Printer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.StoryMode.Walking
{
    public class FadeImageSubPathNode : SubPathNode
    {
        private Image _image;
        [SerializeField] private Color beginColor = new Color();
        [SerializeField] private Color endColor = Color.white;
        [SerializeField] private float disappearDuration = 0.5f;
        public override async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
            gameObject.SetActive(true);
            _image = GetComponent<Image>();
            _image.color = beginColor;
            _image.DOColor(endColor, duration);
        }
        public override Task Disappear()
        {
            _image.DOColor(new Color(), disappearDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            return Task.CompletedTask;
        }
    }
}