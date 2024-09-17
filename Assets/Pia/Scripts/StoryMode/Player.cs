using System;
using Assets.Pia.Scripts.Effect;
using Assets.Pia.Scripts.General;
using Assets.Pia.Scripts.Interface;
using Assets.Pia.Scripts.StoryMode.Walking;
using Assets.Pia.Scripts.UI;
using Default.Scripts.Extension;
using Default.Scripts.Util;
using DG.Tweening;
using Pia.Scripts.StoryMode;
using UniRx;
using UniRx.Triggers;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using GlobalInputBinder = Default.Scripts.Util.GlobalInputBinder;
using Unit = UniRx.Unit;

namespace Assets.Pia.Scripts.StoryMode
{
    public class Player : MonoBehaviour
    {
        public enum LowerAnimationState
        {
            Idle,
            Walk,
            Crouch
        }
        public enum UpperState
        {
            None,
            Bandage,
            Bottle,
            Dagger,
            Driver,
            Nipper,
            Pen,
            Shovel,
            ShovelUse
        }

        [SerializeField] private PlayerUI _playerUI;
        [SerializeField] PathManager pathManager;

        [Header("키 세팅")]
        public KeyCode walkKey = KeyCode.W;
        public KeyCode crouchKey = KeyCode.LeftControl;
        public KeyCode bagKey = KeyCode.Tab;
        public KeyCode guideBookKey = KeyCode.R;
        public KeyCode guideBookPreviousKey = KeyCode.Q;
        public KeyCode guideBookNextKey = KeyCode.E;
        public KeyCode cartridgeKey = KeyCode.F;
        public KeyCode flashlightKey = KeyCode.C;
        public KeyCode letterKey = KeyCode.T;
        public KeyCode secondaryBagKey = KeyCode.G;
        public KeyCode bandageKey = KeyCode.V;
        public KeyCode driverKey = KeyCode.E;
        public KeyCode nipperKey = KeyCode.D;
        public KeyCode penKey = KeyCode.X;
        public KeyCode daggerKey = KeyCode.Y;
        public KeyCode shovelKey = KeyCode.H;
        public KeyCode bottleKey = KeyCode.B;
        public KeyCode stepKey = KeyCode.S;
        public KeyCode stepPedalKey = KeyCode.F7;


        [Header("Tools")]
        [SerializeField] private Light flashLight;
        [SerializeField] private Image letter;
        [SerializeField] private GuideBook guideBook;

        [Header("Body Part")]
        [SerializeField] private Transform head;
        [SerializeField] private Transform body;
        [SerializeField] private Transform upperBody;
        [SerializeField] private Transform arm;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform foot;
        [SerializeField] private Transform crouchArmTransform;
        [SerializeField] private Transform standArmTransform;
        [SerializeField] private Transform crouchHeadTransform1;
        [SerializeField] private Transform crouchHeadTransform2;
        [SerializeField] private Transform standHeadTransform;
        private bool _isCrouching;
        private bool _isMove = false;
        private bool _isInteractable = false;
        private bool _ableToCrouch = true;

        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;



        [Header("Property")]
        [SerializeField] private float sensitiveX = 1.0f;
        [SerializeField] private float sensitiveY = 1.0f;
        [SerializeField] private float defaultFov = 60;

        [Header("   Default")]
        [SerializeField] private float limitRotationY = 0;
        [SerializeField] private float minRotationX = 0;
        [SerializeField] private float maxRotationX = 0;

        [SerializeField] private float walkMinRotationX = 0;
        [SerializeField] private float walkMaxRotationX = 0;

        [SerializeField] private float standUpHeight = 1.0f;
        [SerializeField] private float standUpDuration = 1.0f;

        [SerializeField] private float crouchMinRotationX = 0;
        [SerializeField] private float crouchMaxRotationX = 0;
        [SerializeField] private float crouchLimitRotationY = 0;
        [SerializeField] private float crouchHeight = 1.0f;
        [SerializeField] private float crouchDuration = 1.0f;
        [SerializeField] private float crouchFov = 50;
        [SerializeField] private float speed = 1.0f;



        private float rotationDirectionX = 0;
        private float rotationX = 0;

        private float rotationDirectionY = 0;
        private float rotationY = 0;

        private float previousRotationX;


        public Subject<LowerAnimationState> lowerStateSubject;
        public Subject<UpperState> upperStateSubject;
        private LowerAnimationState currentLowerState;
        private UpperState currentUpperState;

        [Header("Animator")]
        [SerializeField] private Animator lowerAnimator;
        [SerializeField] private Animator bandageAnimator;
        [SerializeField] private Animator bottleAnimator;
        [SerializeField] private Animator daggerAnimator;
        [SerializeField] private Animator driverAnimator;
        [SerializeField] private Animator nippleAnimator;
        [SerializeField] private Animator penAnimator;
        [SerializeField] private Animator shovelAnimator;

        private bool _isPlayActive = false;

        [Header("Dig")]

        [Header("Interaction")]
        [SerializeField] private float checkTargetRayDistance = 1.0f;
        [SerializeField] private PlayerCursor playerCursor;
        public float shovelInterval = 1.0f;
        public float driverInterval = 1.0f;


        private InteractableClass target;


        void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
            initialLocalPosition = mainCamera.transform.localPosition;
            initialLocalRotation = mainCamera.transform.localRotation;
            mainCamera.fieldOfView = defaultFov;
            SetCursorLocked();
            CreateAnimationSubject();
            CreateLowerBodyStream();
            CreateUpperBodyStream();
            CreateCameraStream();
            CreateInteractionStream( "General", UseHand);

            this.UpdateAsObservable()
                .Where(t => _isInteractable)
                .Select(_ => CheckInteractable())
                .Subscribe(t =>
                {
                    if (target != null)
                    {
                        target.OnExit();
                    }
                    if (t != null)
                    {
                        t.OnHover(currentUpperState);
                    }
                    target = t;
                }, null, () =>
                {
                    if (target != null)
                    {
                        target.OnExit();
                    }

                    target = null;
                }).AddTo(gameObject);
        }

        public void CreateInteractionStream(string tag, Action<InteractableClass> interact)
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
                .Select(_ => CheckTarget(tag))
                .Where(_=>_isInteractable)
                .Where(x => x != null)
                .Subscribe(interact.Invoke)
                .AddTo(gameObject);
        }
        public void CreateInteractionStream(IObservable<Unit> takeUntil, string tag, Action<InteractableClass> interact)
        {
            GlobalInputBinder.CreateGetKeyDownStream(KeyCode.Mouse0)
               .TakeUntil(takeUntil)
               .Select(_ => CheckTarget(tag))
               .Where(_ => _isInteractable)
               .Where(x => x != null)
               .Subscribe(interact.Invoke)
               .AddTo(gameObject);
        }
        private void CreateCameraStream()
        {
            GlobalInputBinder.CreateGetAxisStream("Mouse X").Subscribe(RotateCameraY);
            GlobalInputBinder.CreateGetAxisStream("Mouse Y").Subscribe(RotateCameraX);
            this.UpdateAsObservable().Subscribe(_ => RotateHead());
            this.UpdateAsObservable().Select(_ => currentLowerState)
                .DistinctUntilChanged().Subscribe(ShakeCamera);
        }
        private void ShakeCamera(LowerAnimationState state)
        {
            DOTween.Kill("shakeCamera");
            DOTween.Kill("shakeCameraRot");
            mainCamera.transform.localPosition = initialLocalPosition;
            mainCamera.transform.localRotation = initialLocalRotation;
            if (state == LowerAnimationState.Walk)
            {
                mainCamera.transform.DOShakePosition(0.5f, Vector3.up * 0.1f, 0).SetLoops(-1).SetId("shakeCamera").SetEase(Ease.InOutBounce).Restart();
                mainCamera.transform.DOShakeRotation(1.0f, Vector3.forward, 0).SetLoops(-1).SetId("shakeCameraRot").SetEase(Ease.InOutBounce).Restart();
            }
            else
            {
                mainCamera.transform.DOShakeRotation(2.0f, Vector3.forward, 0).SetLoops(-1).SetId("shakeCameraRot").SetEase(Ease.InOutCirc).Restart();
            }
        }
        private void CreateLowerBodyStream()
        {
            this.LateUpdateAsObservable().Subscribe(_ =>
            {
                if (_isMove)
                {
                    lowerStateSubject.OnNext(LowerAnimationState.Walk);
                }
                else if (_isCrouching)
                {
                    lowerStateSubject.OnNext(LowerAnimationState.Crouch);
                }
                else
                {
                    lowerStateSubject.OnNext(LowerAnimationState.Idle);
                }
                _isMove = false;
            });
            CreateWalkingStream();
            CreateCrouchingStream();
        }
        private void CreateCrouchingStream()
        {
            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .SkipWhile(_ => !_isPlayActive)
                .Where(_ => !_isCrouching && _ableToCrouch).Subscribe(_ => Crouch()).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(crouchKey)
                .SkipWhile(_ => !_isPlayActive)
                .Where(_ => _isCrouching && _ableToCrouch).Subscribe(_ => StandUp()).AddTo(gameObject);
        }
        private void CreateWalkingStream()
        {
            this.UpdateAsObservable()
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Select(_ => pathManager.CheckNode(this))
                .Subscribe(_ =>
                {
                    DOTween.Kill("changeDirection");
                    transform.DORotate(Quaternion.LookRotation(pathManager.GetPlayerDirection(this)).eulerAngles, 1.0f).SetId("changeDirection");
                });

            GlobalInputBinder.CreateGetKeyStream(walkKey)
                .Where(_ => StoryModeManager.GetState() == StoryModeManager.State.Walking)
                .Where(_ => !_isCrouching && pathManager.PlayerIsMovable())
                .Subscribe(_ =>
                {
                    MoveForward();
                    _isMove = true;
                }).AddTo(gameObject);
        }

        private void RotateHead()
        {
            head.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
            if (!_isCrouching)
            {
                head.localPosition = new Vector3(rotationY * 0.003f, head.localPosition.y, 0.14f + rotationX * 0.003f);
            }

            DOTween.Kill("followHeadPos");
            upperBody.localRotation = head.localRotation;
            upperBody.DOLocalMove(head.localPosition, 0.2f).SetId("followHeadPos"); ;
        }

        private void CreateAnimationSubject()
        {
            lowerStateSubject = new Subject<LowerAnimationState>();
            upperStateSubject = new Subject<UpperState>();
            lowerStateSubject.Subscribe(AnimateLowerBody);
            upperStateSubject.Subscribe(AnimateUpperBody);
            lowerStateSubject.OnNext(LowerAnimationState.Idle);
            upperStateSubject.OnNext(UpperState.None);
        }

        private void AnimateUpperBody(UpperState state)
        {
            currentUpperState = state;
            bandageAnimator.gameObject.SetActive(false);
            bottleAnimator.gameObject.SetActive(false);
            daggerAnimator.gameObject.SetActive(false);
            driverAnimator.gameObject.SetActive(false);
            nippleAnimator.gameObject.SetActive(false);
            penAnimator.gameObject.SetActive(false);
            shovelAnimator.gameObject.SetActive(false);
            switch (state)
            {
                case UpperState.None:
                    break;
                case UpperState.Bandage:
                    bandageAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Bottle:
                    bottleAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Dagger:
                    daggerAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Driver:
                    driverAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Nipper:
                    nippleAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Pen:
                    penAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.Shovel:
                    shovelAnimator.gameObject.SetActive(true);
                    break;
                case UpperState.ShovelUse:
                    shovelAnimator.gameObject.SetActive(true);
                    shovelAnimator.SetBool("Use", true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        private void AnimateLowerBody(LowerAnimationState state)
        {
            currentLowerState = state;
            lowerAnimator.SetBool("Idle", false);
            lowerAnimator.SetBool("Walk", false);
            lowerAnimator.SetBool("Crouch", false);
            switch (state)
            {
                case LowerAnimationState.Idle:
                    lowerAnimator.SetBool("Idle", true);
                    break;
                case LowerAnimationState.Walk:
                    lowerAnimator.SetBool("Walk", true);
                    break;
                case LowerAnimationState.Crouch:
                    lowerAnimator.SetBool("Crouch", true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void MoveForward()
        {
            transform.Translate(transform.forward * speed * Time.deltaTime);
        }
        public void SetCursorLocked()
        {
            playerCursor.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _isInteractable = false;
        }
        public void SetCursorUnlocked()
        {
            playerCursor.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            _isInteractable = true;
        }
        public void RotateCameraX(float direction)
        {
            if (_ableToCrouch)
            {
                rotationDirectionX = direction;
                rotationX -= direction * sensitiveY;

                if (currentLowerState == LowerAnimationState.Walk)
                {
                    rotationX = Mathf.Clamp(rotationX, walkMinRotationX, walkMaxRotationX);
                }
                else if (currentLowerState == LowerAnimationState.Crouch)
                {
                    rotationX = Mathf.Clamp(rotationX, crouchMinRotationX, crouchMaxRotationX);
                }
                else
                {
                    rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);
                }
            }
        }
        public void RotateCameraY(float direction)
        {
            rotationDirectionY = direction;
            rotationY += direction * sensitiveY;
            if (currentLowerState == LowerAnimationState.Crouch)
            {
                rotationY = Mathf.Clamp(rotationY, -crouchLimitRotationY, crouchLimitRotationY);
            }
            else
            {
                rotationY = Mathf.Clamp(rotationY, -limitRotationY, limitRotationY);
            }

        }
        public void RepositioningThroughFoot(Transform mineTransform)
        {
            var diffrence = mineTransform.position - foot.position;
            transform.position += diffrence;
        }
        private void CreateUpperBodyStream()
        {
            //가방(Tab)
            GlobalInputBinder.CreateGetKeyDownStream(bagKey).SkipWhile(_ => !_isPlayActive).Where(_ => !_playerUI.IsBagOpen()).Subscribe(_ =>
            {
                _playerUI.OpenBag();
            });
            GlobalInputBinder.CreateGetKeyDownStream(bagKey).SkipWhile(_ => !_isPlayActive).Where(_ => _playerUI.IsBagOpen()).Subscribe(_ =>
            {
                _playerUI.CloseBag();
            });

            //탄띠(F)
            GlobalInputBinder.CreateGetKeyDownStream(cartridgeKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnCartridgeKeyPressed());

            //보조가방(G)
            GlobalInputBinder.CreateGetKeyDownStream(secondaryBagKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnSecondaryBagKeyPressed());

            //손전등(C)
            GlobalInputBinder.CreateGetKeyDownStream(flashlightKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnFlashLightKeyPressed());

            //편지(T)
            GlobalInputBinder.CreateGetKeyDownStream(letterKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnLetterKeyPressed());

            //교본(R)
            GlobalInputBinder.CreateGetKeyDownStream(guideBookKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnGuideBookKeyPressed());

            //붕대(V)
            GlobalInputBinder.CreateGetKeyDownStream(bandageKey).SkipWhile(_ => !_isPlayActive).Subscribe(_ => OnBandageKeyPressed());

        }

        private InteractableClass CheckInteractable()
        {
            RaycastHit hit;
            var center = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(center);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * checkTargetRayDistance,Color.red);
#endif
            var isContact = Physics.Raycast(ray.origin, ray.direction, out hit, checkTargetRayDistance);
            if (isContact && hit.transform.GetComponent<InteractableClass>().IsAvailable())
            {
                return hit.transform.GetComponent<InteractableClass>();
            }
            else
            {
                return null;
            }
        }
        private InteractableClass CheckTarget(string tag)
        {
            RaycastHit hit;
            var center = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(center);
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * checkTargetRayDistance,Color.red);
#endif
            var isContact = Physics.Raycast(ray.origin, ray.direction, out hit, checkTargetRayDistance);
            if (isContact && hit.collider.CompareTag(tag) && hit.transform.GetComponent<InteractableClass>().IsAvailable())
            {
                return hit.transform.GetComponent<InteractableClass>();
            }
            else
            {
                return null;
            }
        }

        private void UseHand(InteractableClass t)
        {
            var mouseUpStream = GlobalInputBinder.CreateGetKeyUpStream(KeyCode.Mouse0);
            var beginPosition = Input.mousePosition;
            mouseUpStream.Select(_ => Input.mousePosition)
                .Where(_ => t != null)
                .First()
                .Subscribe(v => t.OnInteract(Vector3.Normalize(v - beginPosition)));
        }

        private void UseDriver(InteractableClass t)
        {
            var driverKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(driverKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);
            var stopUseStream = GlobalInputBinder.CreateGetKeyUpStream(KeyCode.Mouse0);
            var cancelStream = driverKeyReleasedStream.Amb(cartridgeCloseStream).Amb(stopUseStream);
            Observable.Interval(TimeSpan.FromSeconds(driverInterval))
                .Where(_ => t != null)
                .TakeUntil(cancelStream)
                .Subscribe(_ =>
                {
                    t.OnInteract(driverInterval);
                });
        }

        private void UseNipper(InteractableClass t)
        {
            t.OnInteract(this);
        }
        private void UseDagger(InteractableClass t)
        {
            t.OnInteract(this);
        }
        private void UsePen(InteractableClass t)
        {
            t.OnInteract(this);
        }
        private void UseShovel(InteractableClass t)
        {
            var shovelKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(shovelKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);
            var stopUseStream = GlobalInputBinder.CreateGetKeyUpStream(KeyCode.Mouse0);
            var cancelStream = shovelKeyReleasedStream.Amb(secondaryBagCloseStream).Amb(stopUseStream);

            Observable.Interval(TimeSpan.FromSeconds(shovelInterval)).Where(_ => t != null).TakeUntil(cancelStream).Subscribe(_ =>
            {
                t.OnInteract(shovelInterval);
                upperStateSubject.OnNext(UpperState.ShovelUse);
            });
            upperStateSubject.OnNext(UpperState.ShovelUse);
        }
        public void Crouch()
        {
            _ableToCrouch = false;
            _isCrouching = true;
            body.DOLocalMoveY(crouchHeight, crouchDuration).OnComplete(() =>
            {
                _ableToCrouch = true;
                SetCursorUnlocked();
            });
            arm.DOLocalMove(crouchArmTransform.localPosition, crouchDuration);
            if (StoryModeManager.GetState() == StoryModeManager.State.LandMineDirt)
            {
                head.DOLocalMove(crouchHeadTransform1.localPosition, crouchDuration);
            }
            else if (StoryModeManager.GetState() == StoryModeManager.State.LandMine)
            {
                head.DOLocalMove(crouchHeadTransform2.localPosition, crouchDuration);
            }
            mainCamera.DOFieldOfView(crouchFov, crouchDuration);
            DOTween.To(() => rotationX, x => rotationX = x, crouchMaxRotationX, crouchDuration);

        }
        public void StandUp()
        {
            _ableToCrouch = false;
            _isCrouching = false;
            body.DOLocalMoveY(standUpHeight, standUpDuration).OnComplete(() =>
            {
                _ableToCrouch = true;
                SetCursorLocked();
            });
            arm.DOLocalMove(standArmTransform.localPosition, standUpDuration);
            head.DOLocalMove(standHeadTransform.localPosition, crouchDuration);
            mainCamera.DOFieldOfView(defaultFov, standUpDuration);
            DOTween.To(() => rotationX, x => rotationX = x, (maxRotationX + minRotationX) / 2, standUpDuration);

        }
        #region KeyEvent
        #region Cartridge

        private void OnCartridgeKeyPressed()
        {
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            cartridgeCloseStream.First().Subscribe(_ => OnCartridgeKeyReleased());
            GlobalInputBinder.CreateGetKeyDownStream(driverKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(cartridgeCloseStream)
                .Subscribe(_ => OnDriverKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(nipperKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(cartridgeCloseStream)
                .Subscribe(_ => OnNipperKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(penKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(cartridgeCloseStream)
                .Subscribe(_ => OnPenKeyPressed()).AddTo(gameObject);
            _playerUI.OpenCartridge();
        }

        private void OnCartridgeKeyReleased()
        {
            _playerUI.CloseCartridge();
        }

        private void OnPenKeyPressed()
        {
            var penReleaseStream = GlobalInputBinder.CreateGetKeyUpStream(penKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            var cancelStream = penReleaseStream.Amb(cartridgeCloseStream);
            cancelStream.First().Subscribe(_ => OnPenKeyRelaese());
            upperStateSubject.OnNext(UpperState.Pen);
            _playerUI.SetSlotActive(_playerUI.penSlot);
            CreateInteractionStream(cancelStream, "PenHole", UsePen);
        }

  

        private void OnPenKeyRelaese()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.penSlot);
        }

        private void OnNipperKeyPressed()
        {
            var nipperReleaseStream = GlobalInputBinder.CreateGetKeyUpStream(nipperKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);
            var cancelStream = nipperReleaseStream.Amb(cartridgeCloseStream);

            cancelStream.First().Subscribe(_ => OnNipperKeyRelaese());
            upperStateSubject.OnNext(UpperState.Nipper);
            _playerUI.SetSlotActive(_playerUI.nipperSlot);
            CreateInteractionStream(cancelStream, "Spring", UseNipper);
            
        }

        private void OnNipperKeyRelaese()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.nipperSlot);
        }

        private void OnDriverKeyPressed()
        {
            var driverReleaseStream = GlobalInputBinder.CreateGetKeyUpStream(driverKey);
            var cartridgeCloseStream = GlobalInputBinder.CreateGetKeyUpStream(cartridgeKey);

            var cancelStream = driverReleaseStream.Amb(cartridgeCloseStream);

            cancelStream.First().Subscribe(_ => OnDriverKeyRelaese());
            upperStateSubject.OnNext(UpperState.Driver);
            _playerUI.SetSlotActive(_playerUI.driverSlot);

            CreateInteractionStream(cancelStream, "Bolt", UseDriver);
        }
        private void OnDriverKeyRelaese()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.driverSlot);
        }

        #endregion

        #region SecondaryBag;
        private void OnSecondaryBagKeyPressed()
        {
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);

            secondaryBagCloseStream.First().Subscribe(_ => OnSecondaryBagKeyReleased());

            GlobalInputBinder.CreateGetKeyDownStream(daggerKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(secondaryBagCloseStream)
                .Subscribe(_ => OnDaggerKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(shovelKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(secondaryBagCloseStream)
                .Subscribe(_ => OnShovelKeyPressed()).AddTo(gameObject);
            GlobalInputBinder.CreateGetKeyDownStream(bottleKey).Where(_ => currentUpperState == UpperState.None).TakeUntil(secondaryBagCloseStream)
                .Subscribe(_ => OnBottleKeyPressed()).AddTo(gameObject);
            _playerUI.OpenSecondaryBag();
        }
        private void OnSecondaryBagKeyReleased()
        {
            _playerUI.CloseSecondaryBag();
        }

        private void OnDaggerKeyPressed()
        {
            var daggerKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(daggerKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);
            var cancelStream = daggerKeyReleasedStream.Amb(secondaryBagCloseStream);

            cancelStream.First().Subscribe(_ => OnDaggerKeyReleased());
            upperStateSubject.OnNext(UpperState.Dagger);
            _playerUI.SetSlotActive(_playerUI.daggerSlot);

            CreateInteractionStream(cancelStream, "Dagger", UseDagger);
        }

   
        private void OnDaggerKeyReleased()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.daggerSlot);
        }

        private void OnShovelKeyPressed()
        {
            var shovelKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(shovelKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);
            var cancelStream = shovelKeyReleasedStream.Amb(secondaryBagCloseStream);

            cancelStream.First().Subscribe(_ => OnShovelKeyReleased());
            upperStateSubject.OnNext(UpperState.Shovel);
            _playerUI.SetSlotActive(_playerUI.shovelSlot);

            CreateInteractionStream(cancelStream, "Dirt", UseShovel);
        }

        private void OnShovelKeyReleased()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.shovelSlot);
        }

        #endregion
        private void OnBottleKeyPressed()
        {
            var bottleKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(bottleKey);
            var secondaryBagCloseStream = GlobalInputBinder.CreateGetKeyUpStream(secondaryBagKey);

            bottleKeyReleasedStream.Amb(secondaryBagCloseStream)
                .First().Subscribe(_ => OnBottleKeyReleased());
            upperStateSubject.OnNext(UpperState.Bottle);
            _playerUI.SetSlotActive(_playerUI.bottleSlot);
        }
        private void OnBottleKeyReleased()
        {
            upperStateSubject.OnNext(UpperState.None);
            _playerUI.SetSlotInactive(_playerUI.bottleSlot);
        }


        private void OnLetterKeyPressed()
        {
            var letterKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(letterKey);
            letterKeyReleasedStream.First().Subscribe(_ => OnLetterKeyReleased());
            _playerUI.SetSlotActive(_playerUI.letterSlot);
            letter.gameObject.SetActive(true);
        }

        private void OnLetterKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.letterSlot);
            letter.gameObject.SetActive(false);
        }
        private void OnFlashLightKeyPressed()
        {
            var flashLightKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(flashlightKey);
            flashLightKeyReleasedStream.First().Subscribe(_ => OnFlashLightKeyReleased());
            _playerUI.SetSlotActive(_playerUI.flashLightSlot);
            flashLight.enabled = true;
        }
        private void OnFlashLightKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.flashLightSlot);
            flashLight.enabled = false;
        }
        private void OnGuideBookKeyPressed()
        {
            var guideBookKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(guideBookKey);
            guideBookKeyReleasedStream.First().Subscribe(_ => OnGuideBookKeyReleased());
            _playerUI.SetSlotActive(_playerUI.guideBookSlot);
            guideBook.gameObject.SetActive(true);

            GlobalInputBinder.CreateGetKeyDownStream(guideBookPreviousKey)
                .TakeUntil(guideBookKeyReleasedStream)
                .Subscribe(_ =>
                {
                    guideBook.Previous();
                }).AddTo(gameObject);

            GlobalInputBinder.CreateGetKeyDownStream(guideBookNextKey)
                .TakeUntil(guideBookKeyReleasedStream)
                .Subscribe(_ =>
                {
                    guideBook.Next();
                }).AddTo(gameObject);
        }

        private void OnGuideBookKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.guideBookSlot);
            guideBook.gameObject.SetActive(false);
        }

        private void OnBandageKeyPressed()
        {
            var bandageKeyReleasedStream = GlobalInputBinder.CreateGetKeyUpStream(bandageKey);
            bandageKeyReleasedStream.First().Subscribe(_ => OnBandageKeyReleased());
            _playerUI.SetSlotActive(_playerUI.bandageSlot);
            upperStateSubject.OnNext(UpperState.Bandage);
        }

        private void OnBandageKeyReleased()
        {
            _playerUI.SetSlotInactive(_playerUI.bandageSlot);
            upperStateSubject.OnNext(UpperState.None);
        }


        #endregion





        public void ActivePlayerUI()
        {
            _isPlayActive = true;
        }
        public void InactivePlayerUI()
        {
            _isPlayActive = false;
        }
        public void ActiveBagSlot()
        {
            _playerUI.bagSlot.gameObject.SetActive(true);
        }

        public void ActiveHealthBar()
        {
            _playerUI.healthBar.gameObject.SetActive(true);
        }


    }
}

