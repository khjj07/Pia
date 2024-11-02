using System;
using System.Threading;
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

        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                gameObject.SetActive(true);
                _image.DOColor(endColor, duration);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }
    }
}