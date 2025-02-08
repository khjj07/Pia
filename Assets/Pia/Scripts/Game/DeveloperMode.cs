using Assets.Pia.Scripts.Game.Items;
using Default.Scripts.Util;
using Pia.Scripts.Manager;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class DeveloperMode : MonoBehaviour
{
    [SerializeField]
    private bool _developerFlag;
    private bool invincibilityFlag;

    public Canvas deveoperUI;
    public Canvas invincibilityUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var neverDiekeyStream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.F2)
            .Subscribe(_ =>
            {
                invincibilityFlag = !invincibilityFlag;
                StoryModeManager.Instance.SetInvincibility(invincibilityFlag);
                if (!invincibilityFlag)
                {
                    invincibilityUI.gameObject.SetActive(false);
                }
                else
                {
                    invincibilityUI.gameObject.SetActive(true);
                }
            });

        var keyStream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.F1)
            .Subscribe(_ =>
            {
                _developerFlag = !_developerFlag;
                if (!_developerFlag)
                {
                    deveoperUI.gameObject.SetActive(false);
                }
                else
                {
                    deveoperUI.gameObject.SetActive(true);
                }
            });

        var developerStream = this.UpdateAsObservable().Where(_ => _developerFlag);

        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("TitleMenu", 0, GlobalLoadingManager.Mode.None,true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("StoryModeSynopsis", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                PlayerPrefs.DeleteKey("Save");
                StoryModeLoadingManager.Instance.LoadScene("StoryModePlay", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                PlayerPrefs.SetString("Save", "LandMineDirt");
                StoryModeLoadingManager.Instance.LoadScene("StoryModePlay",0,GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha5))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("StoryModeEnding", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha6))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("GameOverMineBomb", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha7))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("GameOverBoar", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha8))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("GameOverEnemy", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha9))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("GameOverAirBomb", 0, GlobalLoadingManager.Mode.None, true);
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha0))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StoryModeLoadingManager.Instance.LoadScene("GameOverHP0", 0, GlobalLoadingManager.Mode.None, true);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
