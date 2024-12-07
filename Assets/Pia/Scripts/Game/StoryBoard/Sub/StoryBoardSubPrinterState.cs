using System;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Printer;
using Default.Scripts.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    public class StoryBoardSubPrinterState : StoryBoardSubState
    {
        private Printer _printer;
        private Image _image;

        [TextArea]
        public string text;
        [SerializeField]
        private bool typeSound = true;
        [Header("Image")]
        [SerializeField] private Color beginColor = new Color();
        [SerializeField] private Color endColor = Color.white;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float speed = 1;
        public void Awake()
        {
            
            _image = GetComponent<Image>();
            if (_image)
            {
                _image.color = beginColor;
            }
            _printer = GetComponentInChildren<Printer>();
            if (typeSound)
            {
                _printer.onBeginPrintEvent.AddListener(() => SoundManager.Play("MP_Typewriter And Bell", 2));
                _printer.onEndPrintEvent.AddListener(() => SoundManager.Stop(2));
            }
            
        }
        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                gameObject.SetActive(true); 
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                if (_image)
                {
                    _image.DOColor(endColor, duration);
                    await Task.Delay((int)(duration * 1000), cancellationTokenSource.Token);
                }
                _printer.SetOriginalText(text);
                await _printer.Print(cancellationTokenSource,speed);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }
    }
}