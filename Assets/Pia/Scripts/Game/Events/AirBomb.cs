﻿using System;
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
        public RectTransform airbombUI;
        public Image airbombGauge;
        public TMP_Text airbombTimerText;

        public RectTransform airbombCheckBar;
        public float positionBoundX = 225;
        public float matchPositionBound = 60;

        private float direction = 1;
        private float balance = 100;

        public float remainTimer = 30.0f;

        public float[] speeds = new float[5] { 5f, 7.5f, 10f, 12.5f, 15f }; // 5단계 속도
        public float[] balanceDecreasementAmounts = new float[5] { 5f, 10f, 15f, 20f, 25f }; // 5단계 감소량
        public float balanceIncreasementAmount = 0.1f;

        public int difficulty;
        private void SetTimer(float f)
        {
            remainTimer = f;
            airbombTimerText.text = "[" + f.ToString("F1") + "]";
        }

        private void UpdateDifficulty()
        {
            difficulty = (int)(30.0f-remainTimer) / 6;
        }
        public override void Act()
        {
            //흔들림 + 미니게임
            var player = StoryModeManager.Instance.GetPlayer();
            player.SetCursorLockedAndInteractable();
            airbombUI.gameObject.SetActive(true);
            Observable.Interval(TimeSpan.FromMilliseconds(100)).TakeWhile(_ => remainTimer > 0 && balance>0)
                .Subscribe(_ =>
                {
                    player.transform.DOShakePosition(0.1f,new Vector3(0.01f,0,0.01f));
                    SetTimer(remainTimer - 0.1f);
                    UpdateDifficulty();
                },null, () =>
                {
                    player.SetCursorUnlocked();
                    airbombUI.gameObject.SetActive(false);
                    if (balance <= 0)
                    {
                        StoryModeManager.GameOver(StoryModeManager.GameOverType.AirBomb);
                    }
                }).AddTo(gameObject);


            this.UpdateAsObservable().TakeWhile(_ => remainTimer > 0 && balance > 0)
                .Subscribe(_ =>
                {
                    SetBalance(balance - balanceDecreasementAmounts[difficulty]*Time.deltaTime);
                    MoveCheckBar();
                });

            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
                .TakeWhile(_ => remainTimer > 0 && balance > 0)
                .Where(_ => CheckIsInBound())
                .Subscribe(_ => SetBalance(balance + balanceIncreasementAmount));

        }

        private bool CheckIsInBound()
        {
            return MathF.Abs(airbombCheckBar.anchoredPosition.x) < matchPositionBound;
        }

        private void SetBalance(float gauge)
        {
            balance = gauge;
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