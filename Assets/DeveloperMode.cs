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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var keyStream = GlobalInputBinder.CreateGetKeyDownStream(KeyCode.F1)
            .Subscribe(_ => _developerFlag = !_developerFlag);

        var developerStream = this.UpdateAsObservable().Where(_ => _developerFlag);

        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StartCoroutine(StoryModeLoadingManager.Instance.Load("TitleMenu", 0, GlobalLoadingManager.Mode.None));
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModeSynopsis", 0,
                    GlobalLoadingManager.Mode.None));
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                PlayerPrefs.DeleteKey("Save");
                StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModePlay", 0, GlobalLoadingManager.Mode.None));
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                PlayerPrefs.SetString("Save", "LandMineDirt");
                StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModePlay",0,GlobalLoadingManager.Mode.None));
            });
        developerStream.Where(_ => Input.GetKeyDown(KeyCode.Alpha5))
            .Subscribe(_ =>
            {
                if (StoryModeManager.Instance)
                {
                    StoryModeManager.Instance.gameOverTokenSource.Cancel();
                }
                StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModeEnding", 0, GlobalLoadingManager.Mode.None));
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
