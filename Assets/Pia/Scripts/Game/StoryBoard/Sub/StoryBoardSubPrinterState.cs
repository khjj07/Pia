using System.Threading.Tasks;
using Default.Scripts.Printer;
using Default.Scripts.Sound;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode.Synopsis.Sub
{
    [RequireComponent(typeof(Printer))]
    public class StoryBoardSubPrinterState : StoryBoardSubState
    {
        [TextArea]
        public string text;
        private Printer _printer;
        [SerializeField]
        private bool typeSound = true;
        public void Awake()
        {
            _printer = GetComponent<Printer>();
            if (typeSound)
            {
                _printer.onBeginPrintEvent.AddListener(() => SoundManager.Play("MP_Typewriter And Bell", 2));
                _printer.onEndPrintEvent.AddListener(() => SoundManager.Stop(2));
            }
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