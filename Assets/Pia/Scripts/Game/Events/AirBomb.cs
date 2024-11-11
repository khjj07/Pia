using System;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.Game.Events
{
    public class AirBomb : EventActor
    {
        public CanvasGroup airbombUI;
        public Image airbombGauge;
        public TMP_Text airbombTimerText;

        public RectTransform airbombCheckBar;

        public float positionBoundX = 225;
        [Header("판정")]
        public float matchPositionBound = 100;

        private float direction = 1;
        private float balance = 100;

        public float remainTimer = 30.0f;

        public float[] speeds = new float[5] { 6, 9, 12, 15, 18 }; // 5단계 속도
        public float[] balanceDecreasementAmounts = new float[5] { 7, 12, 17, 21, 25 }; // 5단계 감소량
        public float balanceIncreasementAmount = 10f;

        public int difficulty;
        private void SetTimer(float f)
        {
            remainTimer = f;
            airbombTimerText.text = "[" + f.ToString("F1") + "]";
        }

        private void UpdateDifficulty()
        {
            difficulty = (int)(30.0f - remainTimer) / 6;
        }
        public override void Act()
        {
            //흔들림 + 미니게임
            var player = StoryModeManager.Instance.GetPlayer();
            player.StandUp();
            player.SetCursorLockedAndInteractable();
            SoundManager.Play("event_startBombing", 6);
            Sequence sequnce = DOTween.Sequence();

            sequnce.AppendInterval(6.0f);

            sequnce.AppendCallback(() =>
            {
                player.mainCamera.DOShakePosition(0.2f, 0.1f).SetLoops(-1).SetId("AirBombPlayerShake");
            });
            sequnce.AppendInterval(3.0f);
            sequnce.AppendCallback(() =>
            {
                airbombUI.gameObject.SetActive(true);
                airbombUI.DOFade(1, 1);
                Observable.Interval(TimeSpan.FromMilliseconds(100)).TakeWhile(_ => remainTimer > 0 && balance > 0)
                    .Subscribe(_ =>
                    {

                        SetTimer(remainTimer - 0.1f);
                        UpdateDifficulty();
                    }, null, () =>
                    {
                        player.SetCursorUnlocked();
                        airbombUI.DOFade(0, 2);
                        player.Crouch();
                        SoundManager.Stop(6);
                        DOTween.Kill("AirBombPlayerShake");
                        if (balance <= 0)
                        {
                            StoryModeManager.GameOver(StoryModeManager.GameOverType.AirBomb);
                        }
                    }).AddTo(gameObject);


                this.UpdateAsObservable().TakeWhile(_ => remainTimer > 0 && balance > 0)
                    .Subscribe(_ =>
                    {
                        SetBalance(balance - balanceDecreasementAmounts[difficulty] * Time.deltaTime);
                        MoveCheckBar();
                    });

                GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
                    .TakeWhile(_ => remainTimer > 0 && balance > 0)
                    .Where(_ => CheckIsInBound())
                    .Subscribe(_ => SetBalance(balance + balanceIncreasementAmount));
            });
            sequnce.AppendInterval(4.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(1.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(2.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(6.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(2.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(3.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(3.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(2.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(1.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(6.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.AppendInterval(2.0f);
            sequnce.Append(player.transform.DOShakePosition(0.1f, 0.5f).SetEase(Ease.InOutElastic));
            sequnce.Play();
        }

        private bool CheckIsInBound()
        {
            return MathF.Abs(airbombCheckBar.anchoredPosition.x) < matchPositionBound;
        }

        private void SetBalance(float gauge)
        {
            balance = Math.Clamp(gauge, 0, 100);
            airbombGauge.fillAmount = balance / 100.0f;
        }

        private void MoveCheckBar()
        {
            if (airbombCheckBar.anchoredPosition.x > positionBoundX || airbombCheckBar.anchoredPosition.x < -positionBoundX)
            {
                direction = -direction;
            }
            DOTween.Kill("checkBar");
            airbombCheckBar.DOAnchorPosX(direction * speeds[difficulty], 0.01f).SetRelative(true).SetId("checkBar");
        }
    }
}