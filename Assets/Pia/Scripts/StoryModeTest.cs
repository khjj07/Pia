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
            var keyStream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.F1)
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
                });
            var developerStream = this.UpdateAsObservable().Where(_ => _developerFlag);
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad0))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad1))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.GetPlayer().Hold(null);
                    StoryModeManager.Instance.GetPlayer().Hold(shovel);
                }
            });

            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad2))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(driver);
                    }
                });

            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad3))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(nipper);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad4))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(dagger);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad5))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(pen);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad6))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(bottle);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad7))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(bandage);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad8))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(letter);
                    }
                });
            developerStream.Where(_ => Input.GetKeyDown(KeyCode.Keypad9))
                .Subscribe(_ =>
                {
                    if (StoryModeManager.Instance)
                    {
                        StoryModeManager.Instance.GetPlayer().Hold(null);
                        StoryModeManager.Instance.GetPlayer().Hold(guideBook);
                    }
                });
        }
    }
        
}