using System;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Pia.Scripts.UI
{
    public class LandMineUI : MonoBehaviour
    {
        public TMP_Text timerText;
        public float timeLimit;
        public float timer;
        public Image blood;

        [SerializeField] private RectTransform generalControlAlert;
        [SerializeField] private RectTransform PedalControlAlert;

        private void SetTimer(float f)
        {
            timer = f;
            timerText.text = "[" + f.ToString("F1") + "]";
            if (timer <= 0)
            {
                StoryModeManager.GameOver(StoryModeManager.GameOverType.MineExplosion);
            }
        }
        private void CreateLandMineStream()
        {
            Observable.Interval(TimeSpan.FromMilliseconds(100)).TakeUntil(StoryModeManager.GetStepStream())
                        .Subscribe(_ => SetTimer(timer - 0.1f)).AddTo(gameObject);
            StoryModeManager.GetStepStream().Take(1).Subscribe(_ =>
            {
                StoryModeManager.Instance.SetInteractionActive(true);
                StoryModeManager.GetStepUpStream()
                    .Subscribe(_ => StoryModeManager.GameOver(StoryModeManager.GameOverType.MineExplosion))
                    .AddTo(StoryModeManager.Instance.gameObject);
                Disappear();

            });
        }

        public void Appear()
        {

            gameObject.SetActive(true);
            SoundManager.Play("StepLandmine", 1);
            blood.DOFade(0.5f,1).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.Linear);
            SetTimer(timeLimit);
            CreateLandMineStream();
            switch (StoryModeManager.GetControlMode())
            {
                case StoryModeManager.ControlMode.General:
                    generalControlAlert.gameObject.SetActive(true);
                    break;
                case StoryModeManager.ControlMode.WithPedal:
                    PedalControlAlert.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void Disappear()
        {
            gameObject.SetActive(false);
        }
    }
}