using System;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using TMPro;
using UniRx;
using UnityEngine;
using UniRx.Triggers;

namespace Assets.Pia.Scripts.Game.Items
{
    public class Bag : MonoBehaviour
    {
        public KeyCode openKey;

        [Header("UI")]
        public TapUI ui;
        [Header("아이템")]
        public Item[] items;

        private bool _isOpen = false;

        public void Initialize(Player player)
        {
            foreach (var item in items)
            {
                item.Initialize(player);
            }
            CreateUseStream();
            _isOpen = false;
            ui.SetActive(false);
        }
        private void CreateUseStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(openKey)
                .Where(_=>StoryModeManager.IsInteractionActive())
                .Subscribe(_ =>
                {
                    if (_isOpen)
                    {
                        Close();
                    }
                    else
                    {
                        Open();
                    }
                }).AddTo(gameObject);
        }

        public void Activate()
        {
           ui.gameObject.SetActive(true);
        }

        public void Open()
        {
            _isOpen = true;
            ui.SetActive(true);
            SoundManager.Play("ui_button", 1);
        }
        public void Close()
        {
            _isOpen = false;
            ui.SetActive(false);
            SoundManager.Play("ui_button", 1);
        }
    }
}