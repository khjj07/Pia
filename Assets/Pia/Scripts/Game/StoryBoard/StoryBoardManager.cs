using System;
using System.Threading;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using Default.Scripts.Util.StatePattern;
using DG.Tweening;
using Pia.Scripts.Dialog;
using Pia.Scripts.Manager;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pia.Scripts.Synopsis
{
    public class StoryBoardManager : StateManager<StoryBoardState>
    {
        [SerializeField] private Image guideNotice;
        [SerializeField] string nextScene;
        private IDisposable goNextStream;
        private IDisposable guideNoticeStream;
      

        public override void Next()
        {
            if (currentIndex >= states.Length - 1)
            {
                goNextStream.Dispose();
                guideNoticeStream.Dispose();
                _cancellationTokenSource.Cancel();
                StartCoroutine(StoryModeLoadingManager.Load(nextScene, 1.0f));
            }
            else
            {
                base.Next();
            }
        }

        public override void Start()
        {
            base.Start();
            SoundManager.StopAll();
            guideNotice.gameObject.SetActive(false);
            goNextStream = this.UpdateAsObservable()
                .Where(_ => currentState.CanGoNext())
                .Where(_ => Input.GetKeyDown(currentState.nextkey))
                .Take(1).Repeat()
                .ThrottleFirst(TimeSpan.FromSeconds(1f))
                .Subscribe(_ =>
                {
                    Next();
                    HideGuideNotice();
                }).AddTo(gameObject);

            guideNoticeStream = this.UpdateAsObservable()
                .SkipWhile(_=> !currentState.CanGoNext())
                .Where(_=>currentState.nextkey==KeyCode.Mouse0)
                .Select(_ => currentState.CanGoNext())
                .DistinctUntilChanged()
                .Subscribe(ChangeGuideNotice)
                .AddTo(gameObject);
        }

        public CancellationToken GetCancellationToken()
        {
            return _cancellationTokenSource.Token;
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
            guideNotice.DOColor(new Color(0,0,0,0), 1f).OnComplete(() =>
            {
                guideNotice.gameObject.SetActive(false);
            });
        }

        private void ShowGuideNotice()
        {
            guideNotice.gameObject.SetActive(true);
            guideNotice.color = new Color();
            guideNotice.DOColor(Color.white, 1f);
        }
    }
}
