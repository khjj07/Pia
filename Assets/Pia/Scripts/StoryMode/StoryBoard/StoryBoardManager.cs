using System;
using Default.Scripts.Util;
using Default.Scripts.Util.StatePattern;
using DG.Tweening;
using Pia.Scripts.Dialog;
using Pia.Scripts.Manager;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Synopsis
{
    public class StoryBoardManager : StateManager<StoryBoardState>
    {
        [SerializeField] private Image guideNotice;

        public override void Next()
        {
            if (currentIndex >= states.Length - 1)
            {
                StartCoroutine(StoryModeLoadingManager.Load("StoryModePlay", 1.0f));
            }
            base.Next();
        }

        public override void Start()
        {
            base.Start();
            guideNotice.gameObject.SetActive(false);

            this.UpdateAsObservable()
                .Take(1).Repeat()
                .Where(_ => currentState.CanGoNext())
                .Where(_ => Input.GetKeyDown(currentState.nextkey))
                .ThrottleFirst(TimeSpan.FromSeconds(1f))
                .Subscribe(_ =>
                {
                    Next();
                    HideGuideNotice();
                }).AddTo(gameObject);

            this.UpdateAsObservable()
                .Where(_=>currentState.nextkey==KeyCode.Mouse0)
                .Select(_ => currentState.CanGoNext())
                .DistinctUntilChanged()
                .Subscribe(ChangeGuideNotice)
                .AddTo(gameObject);
        }

        private void ChangeGuideNotice(bool value)
        {
            if (value)
            {
                ShowGuideNotice();
            }
            else
            {
                HideGuideNotice();
            }
        }

        private void HideGuideNotice()
        {
            guideNotice.DOColor(new Color(0,0,0,0), 0.5f).OnComplete(() =>
            {
                guideNotice.gameObject.SetActive(false);
            });
        }

        private void ShowGuideNotice()
        {
            guideNotice.gameObject.SetActive(true);
            guideNotice.DOColor(Color.white, 0.5f);
        }
    }
}
