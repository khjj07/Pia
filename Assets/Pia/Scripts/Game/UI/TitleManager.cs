using Default.Scripts.Sound;
using Default.Scripts.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Pia.Scripts.Manager
{
    public class TitleManager : MonoBehaviour
    {
        public Button startButton;
        public Button quitButton;

        private void Start()
        {
            startButton.onClick.AddListener(OnStartButtonClick);
            quitButton.onClick.AddListener(OnQuitButtonClick);
            PlayerPrefs.DeleteKey("Save");
            SoundManager.Play("BGM_Title");
        }

        private void OnStartButtonClick()
        {
            SoundManager.Play("ui_Button",1);
            StartCoroutine(StoryModeLoadingManager.Load("StoryModeSynopsis", 1.0f));
        }

        private void OnQuitButtonClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
        }
    }
}