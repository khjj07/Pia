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
    [Serializable]
    public class Bag
    {

        public KeyCode openKey;

        [Header("UI")] 
        public BagUI ui;
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
                .SkipWhile(_=>StoryModeManager.IsInteractionActive())
                .Where(_=>!_isOpen).Subscribe(_ =>Open());
            GlobalInputBinder.CreateGetKeyDownStream(openKey)
                .SkipWhile(_ => StoryModeManager.IsInteractionActive())
                .Where(_ => _isOpen).Subscribe(_ => Close());
        }
        private void Open()
        {
            ui.OpenBag();
            _isOpen=true;
        }
        private void Close()
        {
            ui.CloseBag();
            _isOpen = false;
        }

        public bool IsOpen()
        {
            return _isOpen;
        }
    }
}