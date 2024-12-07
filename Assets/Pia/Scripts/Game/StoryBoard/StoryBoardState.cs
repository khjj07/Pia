using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Pia.Scripts.StoryMode.Synopsis.Sub;
using Default.Scripts.Printer;
using Default.Scripts.Util.StatePattern;
using Pia.Scripts.Manager;
using UnityEngine;

namespace Pia.Scripts.Dialog
{
    public class StoryBoardState : State<StoryBoardState>
    {
        public enum AppearMode
        {
            Sequence,
            Simultaneous
        }

        public KeyCode nextkey = KeyCode.Mouse0;

        protected bool _isAppearing;
        public bool fadeInEnable = true;
        public bool fadeOutEnable = true;

        [SerializeField]
        private StoryBoardSubState[] _subStates;

        [SerializeField] private AppearMode appearMode;

        public void Awake()
        {
            _subStates = GetComponentsInChildren<StoryBoardSubState>();
            foreach (var state in _subStates)
            {
                state.gameObject.SetActive(false);
            }
            _isAppearing = true;
        }

        public virtual bool CanGoNext()
        {
            return !_isAppearing;
        }

        public override async Task OnEnter(CancellationTokenSource tokenSource)
        {
            _isAppearing = true;
            gameObject.SetActive(true);
            if(fadeInEnable)
            {
                await GlobalFadeManager.FadeIn();
            }

            switch (appearMode)
            {
                case AppearMode.Sequence:
                    foreach (var state in _subStates)
                    {
                        state.gameObject.SetActive(true);
                        await state.Appear(tokenSource);
                    }
                    break;
                case AppearMode.Simultaneous:
                    var tasks = new List<Task>();
                    foreach (var state in _subStates)
                    {
                        state.gameObject.SetActive(true);
                        tasks.Add(state.Appear(tokenSource));
                    }

                    await Task.WhenAll(tasks.ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
          
            _isAppearing = false;
        }

        public override async Task OnExit(CancellationTokenSource tokenSource)
        {
            if (fadeOutEnable)
            {
                await GlobalFadeManager.FadeOut();
            }
            foreach (var state in _subStates)
            {
                await state.Disappear(tokenSource);
            }
            gameObject.SetActive(false);
        }
    }
}