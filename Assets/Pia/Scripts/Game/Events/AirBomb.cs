using System;
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
        public RectTransform airbombCheckBar;
        public TMP_Text airbombTimerText;

        public float maxPositionX = 1;
        public float direction = 1;
        public float speed = 1;
        public float balance = 1;

        public float remainTimer = 30.0f;
        public float balanceDecreasement = 0.06f;
        private void SetTimer(float f)
        {
            remainTimer = f;
            airbombTimerText.text = "[" + f.ToString("F1") + "]";
        }

        public override void Act()
        {
            //흔들림 + 미니게임
            airbombUI.gameObject.SetActive(true);
            Observable.Interval(TimeSpan.FromMilliseconds(100)).TakeWhile(_ => remainTimer > 0)
                .Subscribe(_ =>
                {
                    SetTimer(remainTimer - 0.1f);
                    SetBalanceGauge(balance - balanceDecreasement);
                }).AddTo(gameObject);

            this.UpdateAsObservable().TakeWhile(_ => remainTimer > 0)
                .Subscribe(_ =>
                {
                    MoveCheckBar();
                });

        }

        private void SetBalanceGauge(float gauge)
        {
            airbombGauge.fillAmount = gauge;
        }

        private void MoveCheckBar()
        {
            if (airbombCheckBar.anchoredPosition.x > maxPositionX || airbombCheckBar.anchoredPosition.x < 0)
            {
                direction = -direction;
            }
            DOTween.Kill("checkBar");
            airbombCheckBar.DOAnchorPosX(direction * speed, 0.01f).SetRelative(true).SetId("checkBar");
        }
    }
}