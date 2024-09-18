using System.Threading.Tasks;
using Default.Scripts.Printer;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    [RequireComponent(typeof(Printer))]
    public class StoryBoardSubPrinterState : StoryBoardSubState
    {
        [TextArea]
        public string text;
        private Printer _printer;

        public void Awake()
        {
            _printer = GetComponent<Printer>();
        }
        public override async Task Appear()
        {
            await Task.Delay((int)(appearDelay * 1000));
            _printer.SetOriginalText(text);
            gameObject.SetActive(true);
            await _printer.Print();
        }
    }
}