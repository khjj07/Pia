using System;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Printer;
using UnityEngine;

namespace Assets.Pia.Scripts.Path.Sub
{
    public class PrinterSubPathNode : SubPathNode
    {
        [TextArea]
        public string text;
        private Printer _printer;

        private void Awake()
        {
            _printer = GetComponent<Printer>();
        }
        public override async Task Appear(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await Task.Delay((int)(appearDelay * 1000), cancellationTokenSource.Token);
                gameObject.SetActive(true);
                _printer.SetOriginalText(text);
                await _printer.Print(cancellationTokenSource);
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