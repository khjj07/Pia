﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Default.Scripts.Util;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Manager
{
    public class GlobalFadeManager : Singleton<GlobalFadeManager>
    {
        public Image fadeImage;
        public float fadeDuration;
        private void Start()
        {
     
        }

        public static async Task FadeOut()
        {
            try
            {
                Instance.fadeImage.gameObject.SetActive(true);
                Instance.fadeImage.DOColor(Color.black, Instance.fadeDuration);
                await Task.Delay((int)(Instance.fadeDuration * 1000));
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
        }

        public static async Task FadeIn()
        {
            try
            {
                Instance.fadeImage.DOColor(new Color(0, 0, 0, 0), Instance.fadeDuration);
                await Task.Delay((int)(Instance.fadeDuration * 1000));
                Instance.fadeImage.gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Async task was canceled.");
            }
           
        }
    }
}