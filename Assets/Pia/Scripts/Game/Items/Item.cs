﻿using System;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode;
using Assets.Pia.Scripts.UI;
using Assets.Pia.Scripts.UI.Slot;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using Pia.Scripts.StoryMode;
using UniRx;
using UnityEngine;

namespace Assets.Pia.Scripts.Game.Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField]
        private SlotUI slot;
        [SerializeField]
        protected KeyCode itemKey;
        [SerializeField]
        protected string itemSoundName;
        protected bool _isActive=false;
        

        public IDisposable activeStream;
        public Subject<bool> forceInactiveSubject = new Subject<bool>();
        public IObservable<Unit> CreateActiveStream()
        {
            return GlobalInputBinder.CreateGetKeyDownStream(itemKey);
        }
        public IObservable<Unit> CreateInActiveStream()
        {
            return GlobalInputBinder.CreateGetKeyUpStream(itemKey);
        }

        public void DisposeActiveStream()
        {
            if (activeStream != null)
            {
                activeStream.Dispose();
            }
        }
        public virtual void OnActive(Player player)
        {
            _isActive = true; 
            slot.SetActive(true);
            if (itemSoundName != "")
            {
                SoundManager.Play(itemSoundName, 1);
            }
        }
        public virtual void OnInActive(Player player)
        {
            _isActive = false;
            slot.SetActive(false);
        }

        public virtual void Initialize(Player player)
        {
           activeStream  = CreateActiveStream()
                .Where(_ => StoryModeManager.IsInteractionActive())
                .Subscribe(_ =>
                {
                    //player.Hold(this);
                    OnActive(player);
                    var inactiveStream = CreateInActiveStream()
                        .TakeWhile(_ => _isActive)
                        .Take(1)
                        .Subscribe(_ => { OnInActive(player); })
                        .AddTo(gameObject);

                    forceInactiveSubject.Subscribe(_ =>
                    {
                        inactiveStream.Dispose();
                        OnInActive(player);
                    });
                }).AddTo(gameObject);
        }
        public void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                forceInactiveSubject.OnNext(true);
            }
        }
    }
}