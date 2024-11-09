using System;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Pia.Scripts.StoryMode.EventManager;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Bandage : UsableItem
    {
        [SerializeField] private RectTransform bandageUI;
        [SerializeField] private Image progressImage;
        [SerializeField] private float extent = 0.1f;
        [SerializeField] private float decreaseInterval = 0.1f;
        [SerializeField] private float decreaseExtent = 0.03f;

        private bool _isUsing = false;
        private float _progress = 0;
        private IDisposable cancelStream;

        public override void OnUse(Player player)
        {
            if (player.IsBleeeding() && !_isUsing)
            {
                InitializeUI();
                _progress = 0;
                //붕대 미니게임
                var cutStream = GlobalInputBinder.CreateGetKeyDownStream(useKey)
                    .TakeWhile(_ => _progress < 1)
                    .Subscribe(_ =>
                    {
                        TryToCure();
                    });

                var decreaseStream = Observable.Interval(TimeSpan.FromSeconds(decreaseInterval)).TakeWhile(_ => _progress < 1)
                    .Subscribe(_ => SetProgress(_progress - decreaseExtent))
                    .AddTo(gameObject);

                var finishStream = this.UpdateAsObservable()
                    .Where(_ => _progress >= 1.0f)
                    .Take(1).Subscribe(_ =>
                    {
                        Finish(player);
                        player.SetCursorUnlocked();
                        cancelStream.Dispose();
                    }).AddTo(gameObject);



                cancelStream = player.UpdateAsObservable().Where(_ => !_isActive)
                    .Take(1).Subscribe(_ =>
                    {
                        player.SetCursorUnlocked();
                        Cancel();
                        cutStream.Dispose();
                        finishStream.Dispose();
                        decreaseStream.Dispose();
                    });
            }
        }

        private void InitializeUI()
        {
            bandageUI.gameObject.SetActive(true);
            _isUsing = true;
        }

        private void TryToCure()
        {
            SetProgress(_progress + extent);
        }

        private void SetProgress(float progress)
        {
            _progress = Mathf.Clamp(progress, 0, 1);
            progressImage.DOKill();
            progressImage.DOFillAmount(progress, 0.1f);
        }

        private void Finish(Player player)
        {
            if (_progress >= 1.0f)
            {
                player.CureBleed();
            }
            bandageUI.gameObject.SetActive(false);
            _isUsing = false;
        }

        private void Cancel()
        {
            bandageUI.gameObject.SetActive(false);
            _isUsing = false;
        }
    }
}