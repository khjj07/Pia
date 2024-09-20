using System;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Util;
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
        }
        private void CreateUseStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(openKey)
                .SkipWhile(_=>!StoryModeManager.IsInteractionActive())
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

        private void Open()
        {
            Debug.Log("1");
            ui.SetActive(true);
            _isOpen=true;
        }
        private void Close()
        {
            Debug.Log("2");
            _isOpen=false;
            ui.SetActive(false);
        }
    }
}