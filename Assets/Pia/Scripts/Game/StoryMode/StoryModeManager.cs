using System;
using System.Threading;
using Assets.Pia.Scripts.Effect;
using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.Game.UI;
using Assets.Pia.Scripts.General;
using Assets.Pia.Scripts.StoryMode;
using Assets.Pia.Scripts.StoryMode.Walking;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Sound;
using Default.Scripts.Util;
using Pia.Scripts.Dialog;
using Pia.Scripts.Manager;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Unit = UniRx.Unit;

namespace Pia.Scripts.StoryMode
{
    public class StoryModeManager : Default.Scripts.Util.Singleton<StoryModeManager>
    {
        public enum GameOverType
        {
           MineExplosion,
           AirBomb,
           Boar,
           Bleed,
           Enemy,
        }
        public enum ControlMode
        {
            General,
            WithPedal
        }
        public enum State
        {
            Walking,
            LandMineDirt,
            LandMine
        }

        public Subject<State> stateSubject;
        public State currentState;
        public ControlMode controlMode;
        [SerializeField]
        private PathManager _pathManager;
        [SerializeField]
        private LandMineUI _landMineUI;
        [SerializeField]
        private Player _player;
        [SerializeField]
        private LandMine _landMine;

        private bool _isInteractionActive;


        public CancellationTokenSource gameOverTokenSource;
        public void Start()
        {
            InitializeVolumeSetting();
            
            gameOverTokenSource = new CancellationTokenSource();
            stateSubject = new Subject<State>();
            stateSubject.DistinctUntilChanged().Subscribe(x=> currentState=x);
            stateSubject.DistinctUntilChanged().Where(x => x == State.Walking)
                .Subscribe(_ =>
                {
                    _isInteractionActive = false;
                    SoundManager.Play("BGM_bug",3);
                }).AddTo(gameObject);

            stateSubject.DistinctUntilChanged().Where(x => x == State.LandMineDirt)
                .Subscribe(_ =>
                {
                    controlMode = GlobalConfiguration.Instance.GetPedalUse()
                        ? ControlMode.WithPedal
                        : ControlMode.General;
                    _isInteractionActive = false;
                    SoundManager.Play("MP_Nightime", 0);
                    SoundManager.Play("BGM_bug", 3);
                    SoundManager.Play("StepLandmine", 1);
                    PlayerPrefs.SetString("Save", "LandMineDirt");
                    _player.OnStepMine();
                    _player.SetCursorLocked();
                    _landMineUI.Appear();
                    _player.UpdateAsObservable()
                        .TakeWhile(_ => currentState == State.LandMineDirt)
                        .Where(_ => _landMine.IsAvailable())
                        .Subscribe(_ =>
                        {
                            SetState(State.LandMine);
                            _player.Crouch();
                        }).AddTo(_player.gameObject);

                    _player.UpdateAsObservable()
                        .TakeWhile(_ => currentState == State.LandMineDirt)
                        .Subscribe(_ => _player.RepositioningThroughFoot(_landMine.Dirt.top))
                        .AddTo(_player.gameObject);
                }).AddTo(gameObject);
            CheckSaveFlag();
        }

        private void InitializeVolumeSetting()
        {
            GlobalConfiguration.Instance.SetFog(true);
        }

        public Player GetPlayer()
        {
            return _player;
        }
        private void CheckSaveFlag()
        {
            if (PlayerPrefs.HasKey("Save"))
            {
                if (PlayerPrefs.GetString("Save") == "LandMineDirt")
                {
                    SetState(State.LandMineDirt);
                }
                else
                {
                    SetState(State.Walking);
                }
            }
        }

        public static void SetState(State state)
        {
            Instance.stateSubject.OnNext(state);
        }
        public static State GetState()
        {
            return Instance.currentState;
        }
        public static ControlMode GetControlMode()
        {
           return Instance.controlMode;
        }

        public static CancellationTokenSource GetGameOverTokenSource()
        {
           return Instance.gameOverTokenSource;
        }
        public static IObservable<Unit> GetStepStream()
        {
            switch (GetControlMode())
            {
                case ControlMode.General:
                    return Instance.UpdateAsObservable().Where(_=>Input.GetKey(Instance._player.stepKey));
                case ControlMode.WithPedal:
                    return Instance.UpdateAsObservable().Where(_=>Input.GetKey(Instance._player.stepKey) && Input.GetKey(Instance._player.stepPedalKey));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

      
        public static IObservable<Unit> GetStepUpStream()
        {
            switch (GetControlMode())
            {
                case ControlMode.General:
                    return Instance.UpdateAsObservable().Where(_ => Input.GetKeyUp(Instance._player.stepKey));
                case ControlMode.WithPedal:
                    return Instance.UpdateAsObservable().Where(_ => Input.GetKeyUp(Instance._player.stepKey) || Input.GetKeyUp(Instance._player.stepPedalKey));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void GameOver(GameOverType type)
        {
            SoundManager.StopAll();
            Instance._pathManager.StopPrintProcess();
            Instance.gameOverTokenSource.Cancel();
            Instance.GoToGameOverScene(type);
        }

        public void GoToGameOverScene(GameOverType type)
        {
            switch (type)
            {
                case GameOverType.MineExplosion:
                    StartCoroutine(StoryModeLoadingManager.Instance.Load("GameOverMineBomb",0,GlobalLoadingManager.Mode.None));
                    break;
                case GameOverType.AirBomb:
                    StartCoroutine(StoryModeLoadingManager.Instance.Load("GameOverAirBomb",0, GlobalLoadingManager.Mode.None));
                    break;
                case GameOverType.Boar:
                    StartCoroutine(StoryModeLoadingManager.Instance.Load("GameOverBoar", 0, GlobalLoadingManager.Mode.None));
                    break;
                case GameOverType.Bleed:
                    StartCoroutine(StoryModeLoadingManager.Instance.Load("GameOverHP0", 0, GlobalLoadingManager.Mode.None));
                    break;
                case GameOverType.Enemy:
                    StartCoroutine(StoryModeLoadingManager.Instance.Load("GameOverEnemy", 0, GlobalLoadingManager.Mode.None));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            
        }

        public void GameClear()
        {
            StartCoroutine(StoryModeLoadingManager.Instance.Load("StoryModeEnding", 1.0f)); ;
        }

        public static bool IsInteractionActive()
        {
            return Instance._isInteractionActive;
        }

        public void SetInteractionActive(bool value)
        {
            _isInteractionActive = value;
        }
    }
}