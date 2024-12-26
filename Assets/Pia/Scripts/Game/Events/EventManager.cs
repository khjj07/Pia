using System;
using System.Threading.Tasks;
using Assets.Pia.Scripts.Game.Events;
using Default.Scripts.Printer;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UnityEngine;
using UnityEngine.UI;

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
            AirBomb
        }
        [Header("Event")]
        [SerializeField]
        private Printer _eventPrinter;
        [SerializeField]
        private Image _eventPrinterFrame;
        [SerializeField]
        private float duration;

        [SerializeField] private EventActor boar;
        [SerializeField] private EventActor enemy;
        [SerializeField] private EventActor airBomb;

        [SerializeField,TextArea] private string[] eventText;

        private async Task PrintEvent(Event e)
        {
            try
            {
                Debug.Log(eventText[(int)e]);
                _eventPrinterFrame.gameObject.SetActive(true);
                if (_eventPrinter.IsPrinting())
                {
                    _eventPrinter.Skip();
                    await _eventPrinter.Disappear(StoryModeManager.GetGameOverTokenSource());
                }
                _eventPrinter.SetOriginalText(eventText[(int)e]);
                await _eventPrinter.Print(StoryModeManager.GetGameOverTokenSource());
                await Task.Delay((int)(duration * 1000));
                if (!_eventPrinter.IsPrinting())
                {
                    await _eventPrinter.Disappear(StoryModeManager.GetGameOverTokenSource());
                }
                _eventPrinterFrame.gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }

        }

        public static void InvokeEvent(Event e)
        {
            Instance.PrintEvent(e);
            switch (e)
            {
                case Event.Boar:
                    Instance.boar.Act();
                    break;
                case Event.Enemy:
                    Instance.enemy.Act();
                    break;
                case Event.AirBomb:
                    Instance.airBomb.Act();
                    break;
           }
        }
    }
}
