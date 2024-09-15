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
        public override async Task Appear()
        {
         
            await Task.Delay((int)(appearDelay * 1000));
            gameObject.SetActive(true);
            _printer.SetOriginalText(text);
            await _printer.Print();
        }

        public override async Task Disappear()
        {
            await _printer.Disappear();
            gameObject.SetActive(false);
        }
    }
}