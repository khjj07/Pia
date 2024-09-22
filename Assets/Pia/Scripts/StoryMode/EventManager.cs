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
        }

        public static async Task PrintEvent(Event e)
        {
            if (Instance._eventPrinter.IsPrinting())
            {
                Instance._eventPrinter.Skip();
                await Instance._eventPrinter.Disappear();
            }
            Debug.Log(Instance.eventText[(int)e]);
            Instance._eventPrinter.SetOriginalText(Instance.eventText[(int)e]);
            await Instance._eventPrinter.Print();
            await Task.Delay((int)(Instance.duration * 1000));
            if (!Instance._eventPrinter.IsPrinting())
            {
                await Instance._eventPrinter.Disappear();
            }
        }
    }
}
