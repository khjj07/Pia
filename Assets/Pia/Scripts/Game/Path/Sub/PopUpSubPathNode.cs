using System;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Path.Sub
{
    public class PopUpSubPathNode : SubPathNode
    {
        [SerializeField] private float duration;
        [SerializeField] private Ease appearEase;
        [SerializeField] private Ease disappearEase;
        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                gameObject.SetActive(true);
                transform.DOScale(Vector3.one, duration).SetEase(appearEase);

            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }

        public override Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
            transform.DOScale(Vector3.zero, duration).SetEase(disappearEase).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            return Task.CompletedTask;
        }
    }
}