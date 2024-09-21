using System;
using System.Threading.Tasks;
using Default.Scripts.Printer;
using Default.Scripts.Util;
using UnityEngine;

namespace Assets.Pia.Scripts.StoryMode
{
    public class EventManager : Singleton<EventManager>
    {
        public enum Event
        {
            HP80,
            HP60,
            HP40,
            HP20,
            HP10,
            Boar,
            Enemy,
            Bleed1,
            Bleed2,
            Bombing
        }
        [Header("Event")]
        private Printer _eventPrinter;
        [SerializeField]
        private float duration;

        [SerializeField,TextArea] private string[] eventText;

        public void Start()
        {
            _eventPrinter = GetComponent<Printer>();
            gameObject.SetActive(false);
        }

        public async void PrintEvent(Event e)
        {
            await _eventPrinter.Disappear();
            _eventPrinter.StopPrinting();
            _eventPrinter.SetOriginalText(eventText[(int)e]);
            await _eventPrinter.Print();
            await Task.Delay((int)(duration * 1000));
            await _eventPrinter.Disappear();
        }
    }
}
