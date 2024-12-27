using Assets.Pia.Scripts.Game.Items;
using Default.Scripts.Util;
using Pia.Scripts.Manager;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts
{
    public class StoryModeTest : MonoBehaviour
    {
        public HoldableItem shovel;
        public HoldableItem driver;
        public HoldableItem nipper;
        public HoldableItem dagger;
        public HoldableItem pen;
        public HoldableItem bottle;
        public HoldableItem bandage;
        public HoldableItem letter;
        public HoldableItem guideBook;
        public Canvas testUI;
        private bool _developerFlag;

        public void Start()
        {
            var keyStream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.F3)
                .Subscribe(_ =>
                {
                    _developerFlag = !_developerFlag;
                    if (!_developerFlag)
                    {
                        testUI.gameObject.SetActive(false);
                    }
                    else
                    {
                        testUI.gameObject.SetActive(true);
                    }
                }).AddTo(gameObject);
            var developerStream = this.UpdateAsObservable().Where(_ => _developerFlag);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad0))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad1))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.GetPlayer().Hold(null);
                    StoryModeManager.Instance.GetPlayer().Hold(shovel);
                }
            }).AddTo(gameObject);

            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad2))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(driver);
                    }
                }).AddTo(gameObject);

            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad3))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(nipper);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad4))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(dagger);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad5))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(pen);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad6))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(bottle);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad7))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(bandage);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad8))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(letter);
                    }
                }).AddTo(gameObject);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad9))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(guideBook);
                    }
                }).AddTo(gameObject);
        }
    }
        
}