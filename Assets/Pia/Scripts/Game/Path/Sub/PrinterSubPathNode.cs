using System;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Printer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Path.Sub
{
    public class PrinterSubPathNode : SubPathNode
    {
        [TextArea]
        public string text;
        private Printer _printer;

        private Image _image;
        [Header("Image")]
        [SerializeField] private Color beginColor = new Color();
        [SerializeField] private Color endColor = Color.white;
        [SerializeField] private float disappearDuration = 0.5f;
        [SerializeField] private float printSpeed = 1;

        private void Awake()
        {
            _printer = GetComponentInChildren<Printer>();
            _image = GetComponent<Image>();
            if (_image)
            {
                _image.color = beginColor;
            }
        }
        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                gameObject.SetActive(true);
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
               _image = GetComponent<Image>();
                if (_image)
                {
                    _image.DOColor(endColor, duration);
                    await Task.Delay((int)(duration * 1000), cancellationTokenSource.Token);
                }
                _printer.SetOriginalText(text);
                await _printer.Print(cancellationTokenSource,printSpeed);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }

        public override async Task Disappear(CancellationTokenSource cancellationTokenSource)
        {
            await _printer.Disappear(cancellationTokenSource);
            gameObject.SetActive(false);
        }
    }
}