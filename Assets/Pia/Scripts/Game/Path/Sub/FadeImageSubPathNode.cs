using System;
using System.Reflection;
using System.Threading;
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
        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                gameObject.SetActive(true);
                _image = GetComponent<Image>();
                _image.color = beginColor;
                _image.DOColor(endColor, duration);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }
        public override Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
            _image.DOColor(new Color(), disappearDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            return Task.CompletedTask;
        }
    }
}