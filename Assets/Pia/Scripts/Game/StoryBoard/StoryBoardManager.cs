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

        [SerializeField] private KeyCode skipKey;
        [SerializeField] private CanvasGroup skipUI;
        [SerializeField] private float skipDelay = 1.0f;
        [SerializeField] private float skipAmount = 0f;
        [SerializeField] private float fillInterval = 0.1f;
        [SerializeField] private Image fillImage;

        private bool finishFlag = false;
        private IDisposable skipStream;
        private bool nextFlag = true;

        public override void Next()
        {
            if (currentIndex >= states.Length - 1)
            {
                goNextStream.Dispose();
                guideNoticeStream.Dispose();
                _cancellationTokenSource.Cancel();
                StoryModeLoadingManager.Instance.sceneSubject.OnNext(nextScene);
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

            GlobalInputBinder.CreateGetKeyDownStream(skipKey).Subscribe(_ =>
            {
                skipUI.DOKill();
                skipUI.DOFade(1, 0.5f);
                if (skipStream is not null)
                {
                    skipStream.Dispose();
                }

                skipStream = Observable.Interval(TimeSpan.FromSeconds(fillInterval)).Subscribe(_ =>
                {
                    skipAmount += fillInterval * skipDelay;
                    fillImage.fillAmount = skipAmount;
                    if (skipAmount >= 1.0f && !finishFlag)
                    {
                        finishFlag = true;
                        goNextStream.Dispose();
                        guideNoticeStream.Dispose();
                        _cancellationTokenSource.Cancel();
                        SoundManager.Stop(2);
                        SoundManager.Play("MP_Nightime",0);
                        StoryModeLoadingManager.Instance.sceneSubject.OnNext(nextScene);
                    }
                }).AddTo(gameObject);
            }).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyUpStream(skipKey).Subscribe(_ =>
            {
                skipUI.DOKill();
                skipUI.DOFade(0, 0.5f);
                if (skipStream is not null)
                {
                    skipStream.Dispose();
                }
                skipStream = Observable.Interval(TimeSpan.FromSeconds(fillInterval))
                    .TakeWhile(_ => skipAmount > 0)
                    .Subscribe(_ =>
                    {
                        skipAmount -= fillInterval * skipDelay;
                        fillImage.fillAmount = skipAmount;
                    }).AddTo(gameObject);
            }).AddTo(gameObject);


            goNextStream = this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(currentState.nextkey))
                .ThrottleFirst(TimeSpan.FromSeconds(1f))
                .Where(_ => currentState.CanGoNext() && nextFlag)
                .Take(1).Repeat()
                .Subscribe(_ =>
                {
                    Next();
                    nextFlag = false;
                    HideGuideNotice();
                    this.UpdateAsObservable()
                        .SkipWhile(_ => currentState.CanGoNext())
                        .Take(1)
                        .Subscribe(_ => nextFlag = true)
                        .AddTo(gameObject);

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
