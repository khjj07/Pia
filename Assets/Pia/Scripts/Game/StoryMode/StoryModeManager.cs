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
        public State currentState = State.Walking;
        public ControlMode controlMode = ControlMode.General;
        [SerializeField]
        private PathManager _pathManager;
        [SerializeField]
        private LandMineUI _landMineUI;
        [SerializeField]
        private LandMine _landMine;

        [SerializeField]
        private OptionManager optionManager;

        private Player _player;
        private bool _isInteractionActive;


        public CancellationTokenSource gameOverTokenSource;
        private bool _invincibility;

        public void Awake()
        {
            stateSubject = new Subject<State>();
            gameOverTokenSource = new CancellationTokenSource();
        }
        public void Start()
        {
            InitializeVolumeSetting();
            _player = Player.Instance;
         
            stateSubject.DistinctUntilChanged().Subscribe(x => currentState = x);
            stateSubject.DistinctUntilChanged().Where(x => x == State.Walking)
                .Subscribe(_ =>
                {
                    _isInteractionActive = false;
                    SoundManager.Play("BGM_bug", 3);
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
                    _player.bag.Close();
                    _player.SetCursorLocked();
                    _landMineUI.Appear();
                    _player.UpdateAsObservable()
                        .TakeWhile(_ => currentState == State.LandMineDirt)
                        .Where(_ => _landMine.IsAvailable())
                        .Subscribe(_ =>
                        {
                            SetState(State.LandMine);
                            _player.Crouch();
                        }).AddTo(gameObject);

                    _player.UpdateAsObservable()
                        .TakeWhile(_ => currentState == State.LandMineDirt)
                        .Subscribe(_ => _player.RepositioningThroughFoot(_landMine.Dirt.top))
                        .AddTo(gameObject);
                }).AddTo(gameObject);
            _player.Initialize(_pathManager);
            CheckSaveFlag();
            optionManager.Initialize();
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Escape)
                .Where(_ => !optionManager._isOpen && GetState() == State.Walking)
                .Subscribe(_ =>
                {
                    _player.SetCursorUnlocked();
                    _player.SetMovable(false);
                    optionManager.Open();
                }).AddTo(gameObject);

            optionManager.optionConfirmButton.onClick.AddListener(() =>
            {
                _player.SetCursorLocked();
                _player.SetMovable(true);
                GlobalConfiguration.Instance.SetFog(true);
            });

        }

        private void InitializeVolumeSetting()
        {
            GlobalConfiguration.Instance.SetFog(true);
        }

        public Player GetPlayer()
        {
            return _player;
        }

        public PathManager GetPathManager()
        {
            return _pathManager;
        }

        private void CheckSaveFlag()
        {
            if (PlayerPrefs.HasKey("Save"))
            {
                if (PlayerPrefs.GetString("Save") == "LandMineDirt")
                {
                    SetState(State.LandMineDirt);
                }
            }
            else
            {
                SetState(State.Walking);
            }
        }

        public void SetState(State state)
        {
            stateSubject.OnNext(state);
        }

        public State GetState()
        {
            return currentState;
        }
        public ControlMode GetControlMode()
        {
            return controlMode;
        }

        public static CancellationTokenSource GetGameOverTokenSource()
        {
            return Instance.gameOverTokenSource;
        }
        public IObservable<Unit> GetStepStream()
        {
            switch (GetControlMode())
            {
                case ControlMode.General:
                    return Instance.UpdateAsObservable().Where(_ => Input.GetKey(Instance._player.stepKey));
                case ControlMode.WithPedal:
                    return Instance.UpdateAsObservable().Where(_ => Input.GetKey(Instance._player.stepKey) && Input.GetKey(Instance._player.stepPedalKey));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public IObservable<Unit> GetStepUpStream()
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
            if (!_invincibility)
            {
                switch (type)
                {
                    case GameOverType.MineExplosion:
                        StoryModeLoadingManager.Instance.LoadScene("GameOverMineBomb", 0, GlobalLoadingManager.Mode.None, true);
                        break;
                    case GameOverType.AirBomb:
                        StoryModeLoadingManager.Instance.LoadScene("GameOverAirBomb", 0, GlobalLoadingManager.Mode.None, true);
                        break;
                    case GameOverType.Boar:
                        StoryModeLoadingManager.Instance.LoadScene("GameOverBoar", 0, GlobalLoadingManager.Mode.None, true);
                        break;
                    case GameOverType.Bleed:
                        StoryModeLoadingManager.Instance.LoadScene("GameOverHP0", 0, GlobalLoadingManager.Mode.None, true);
                        break;
                    case GameOverType.Enemy:
                        StoryModeLoadingManager.Instance.LoadScene("GameOverEnemy", 0, GlobalLoadingManager.Mode.None, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        public void GameClear()
        {
            StoryModeLoadingManager.Instance.LoadScene("StoryModeEnding", 1.0f);
        }

        public static bool IsInteractionActive()
        {
            return Instance._isInteractionActive;
        }

        public void SetInteractionActive(bool value)
        {
            _isInteractionActive = value;
        }

        public void OpenBag()
        {
            _isInteractionActive = true;
            _player.ActiveBagSlot();
            _player.bag.Open();
        }

        public void CloseBag()
        {
            _player.bag.Close();
        }

        public void SetInvincibility(bool invincibilityFlag)
        {
            _invincibility = invincibilityFlag;
        }
    }
}